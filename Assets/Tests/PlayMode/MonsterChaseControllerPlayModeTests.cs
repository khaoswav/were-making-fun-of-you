using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WereMakingFunOfYou.Tests.PlayMode
{
    public sealed class MonsterChaseControllerPlayModeTests
    {
        private GameObject monsterObject;
        private GameObject targetObject;
        private MonsterChaseController monster;

        [TearDown]
        public void TearDown()
        {
            if (monsterObject != null)
            {
                if (Application.isPlaying) Object.Destroy(monsterObject);
                else Object.DestroyImmediate(monsterObject);
            }

            if (targetObject != null)
            {
                if (Application.isPlaying) Object.Destroy(targetObject);
                else Object.DestroyImmediate(targetObject);
            }
        }

        [UnityTest]
        public IEnumerator CatchCallbackFiresWhenTargetEntersCatchRadius()
        {
            CreateMonsterAndTarget(new Vector3(0f, 0f, 0.6f));
            int caughtCount = 0;

            monster.BeginChase(targetObject.transform, Vector3.zero, () => caughtCount++, null);
            yield return null;
            yield return null;

            Assert.That(caughtCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator StopChasePreventsFurtherMovement()
        {
            CreateMonsterAndTarget(new Vector3(0f, 0f, 50f));
            monster.BeginChase(targetObject.transform, new Vector3(0f, 0f, 50f), null, null);

            yield return new WaitForSeconds(0.25f);
            monster.StopChase();
            Vector3 stoppedPosition = monsterObject.transform.position;

            yield return new WaitForSeconds(0.25f);

            Assert.That(Vector3.Distance(monsterObject.transform.position, stoppedPosition), Is.LessThan(0.01f));
        }

        [UnityTest]
        public IEnumerator ChaseStumblesThenRaisesEnragedCallbackAndMovesFaster()
        {
            CreateMonsterAndTarget(new Vector3(0f, 0f, 100f));
            int enragedCount = 0;
            monster.BeginChase(
                targetObject.transform,
                new Vector3(0f, 0f, 2f),
                null,
                () => enragedCount++);

            yield return new WaitForSeconds(7.1f);

            Assert.That(enragedCount, Is.EqualTo(1), "The stumble recovery should enter the enraged phase once.");
            float positionBeforeEnragedSample = monsterObject.transform.position.z;

            yield return new WaitForSeconds(0.35f);

            float enragedDistance = monsterObject.transform.position.z - positionBeforeEnragedSample;
            Assert.That(enragedDistance, Is.GreaterThan(0.7f), "The monster should resume movement at its enraged speed.");
        }

        private void CreateMonsterAndTarget(Vector3 targetPosition)
        {
            monsterObject = new GameObject("PlayMode Test Monster");
            monsterObject.AddComponent<CharacterController>();

            GameObject visuals = new GameObject("Monster Visuals");
            visuals.transform.SetParent(monsterObject.transform, false);

            monster = monsterObject.AddComponent<MonsterChaseController>();

            targetObject = new GameObject("PlayMode Test Target");
            targetObject.transform.position = targetPosition;
        }
    }
}
