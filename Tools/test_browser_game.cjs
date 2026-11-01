const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const source = fs.readFileSync('browser/game.js', 'utf8');
const elements = new Map();
const listeners = {};
const gradient = { addColorStop() {} };
const ctx = { fillRect(){}, createLinearGradient(){return gradient;}, beginPath(){}, ellipse(){}, fill(){}, fillText(){}, set fillStyle(v){}, set shadowColor(v){}, set shadowBlur(v){}, set font(v){}, set textAlign(v){} };
function element(id) {
  if (!elements.has(id)) elements.set(id, { id, textContent:'', onclick:null, classList:{add(){},remove(){},contains(){return false}}, style:{}, addEventListener(){}, requestPointerLock(){}, getContext(){return ctx} });
  return elements.get(id);
}
const storage = new Map();
const document = { pointerLockElement:null, getElementById:element, addEventListener(k,fn){listeners['document:'+k]=fn;}, exitPointerLock(){}, };
const window = { location:{search:'?test=1'}, addEventListener(k,fn){listeners[k]=fn;}, AudioContext:null, webkitAudioContext:null };
const sandbox = { window, document, performance:{now:()=>1000}, requestAnimationFrame(){}, setTimeout(){}, localStorage:{getItem:k=>storage.get(k)||null,setItem:(k,v)=>storage.set(k,String(v))}, console };
vm.runInNewContext(source, sandbox, {timeout:3000});
const api=window.__WMFO_TEST__;
assert.ok(api, 'test API is exposed only in test mode');
const {chapters,state,loadLevel,interact,findTarget}=api;
function reachable(map,start,target){
 const h=map.length,w=map[0].length,q=[start],seen=new Set([start.join(',')]),dirs=[[1,0],[-1,0],[0,1],[0,-1]];
 for(let i=0;i<q.length;i++){const [x,y]=q[i];if(x===target[0]&&y===target[1])return true;
  for(const [dx,dy] of dirs){const nx=x+dx,ny=y+dy,k=nx+','+ny;if(nx>=0&&ny>=0&&nx<w&&ny<h&&map[ny][nx]===0&&!seen.has(k)){seen.add(k);q.push([nx,ny]);}}
 } return false;
}
let objectCount=0, noteCount=0;
for(let i=0;i<chapters.length;i++){
 loadLevel(i);const c=chapters[i],map=state.walls,start=[Math.floor(c.spawn[0]),Math.floor(c.spawn[1])];
 const count=state.objects.filter(o=>!['exit','secret','note','ending','ending2'].includes(o.kind)).length;
 assert.equal(count,c.need,'chapter '+(i+1)+' required objective count');
 for(const o of state.objects){const cell=[Math.floor(o.x),Math.floor(o.y)];assert.ok(reachable(map,start,cell),'chapter '+(i+1)+' object unreachable: '+o.label);objectCount++;if(o.kind==='note'){assert.equal(o.text.length,2);noteCount++;}}
 if(state.enemy){assert.equal(map[Math.floor(state.enemy.y)][Math.floor(state.enemy.x)],0,'enemy spawned in wall');assert.ok(reachable(map,start,[Math.floor(state.enemy.x),Math.floor(state.enemy.y)]),'enemy inaccessible');}
}
assert.equal(chapters.length,8);
assert.equal(noteCount,16,'sixteen optional story notes');
loadLevel(0);
const badge=state.objects.find(o=>o.kind==='badge');
state.player.x=badge.x;state.player.y=badge.y;state.player.a=0;
assert.equal(findTarget(),badge);interact();assert.equal(state.collected,1);
loadLevel(7);const secret=state.objects.find(o=>o.kind==='secret');state.player.x=secret.x;state.player.y=secret.y;state.player.a=0;interact();assert.equal(state.ending,'secret');
console.log('Browser gameplay tests passed: 8 chapters, '+objectCount+' reachable interactables, 16 bilingual notes, objective counts, enemy spawn/path access, pickup interaction, secret ending.');
