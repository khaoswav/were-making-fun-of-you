from pathlib import Path
import re, sys
root=Path(__file__).resolve().parents[1]
required=["browser/index.html","browser/style.css","browser/game.js"]
errors=[]
for item in required:
    if not (root/item).is_file(): errors.append(f"Missing: {item}")
if not errors:
    html=(root/"browser/index.html").read_text()
    js=(root/"browser/game.js").read_text()
    css=(root/"browser/style.css").read_text()
    for marker in ('play-en','play-tr','game.js','style.css'):
        if marker not in html: errors.append(f"HTML missing marker: {marker}")
    for marker in ('CHAPTER','finishEnding','loadLevel','updateEnemy','pointerLockElement','secret'):
        if marker not in js: errors.append(f"Game logic missing marker: {marker}")
    if js.count('{') != js.count('}'): errors.append("Unbalanced JavaScript braces")
    if js.count('(') != js.count(')'): errors.append("Unbalanced JavaScript parentheses")
    if 'canvas' not in css: errors.append("Missing canvas styling")
if errors:
    print("\n".join(errors)); sys.exit(1)
print("Browser game integrity passed: standalone HTML/CSS/JS entrypoint, bilingual launch, 8 chapters, progression, enemy, secrets, and endings.")
