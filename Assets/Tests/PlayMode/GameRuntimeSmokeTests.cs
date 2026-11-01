using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WereMakingFunOfYou.Tests.PlayMode
{
    public sealed class GameRuntimeSmokeTests
    {
        [UnityTest]
        public IEnumerator RuntimeBootstrapCreatesExactlyOneWorld()
        {
            yield return null;
            yield return null;

            GameDirector[] directors = Object.FindObjectsByType<GameDirector>(FindObjectsSortMode.None);
            Assert.That(directors.Length, Is.EqualTo(1), "Runtime bootstrap should create exactly one GameDirector.");
            Assert.That(GameDirector.Instance, Is.SameAs(directors[0]));
            Assert.That(directors[0].GetComponent<HorrorEncounterDirector>(), Is.Not.Null);
            Assert.That(GameObject.FindGameObjectWithTag("Player"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator WrongDoorAttemptStillRaisesStageTwoBehaviorEvent()
        {
            yield return null;
            yield return null;

            GameDirector game = GameDirector.Instance;
            Assert.That(game, Is.Not.Null, "The runtime bootstrap must be active before this test.");
            HorrorEncounterDirector encounters = game.GetComponent<HorrorEncounterDirector>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Assert.That(encounters, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            PlayerBehaviorTracker tracker = player.GetComponent<PlayerBehaviorTracker>();
            Assert.That(tracker, Is.Not.Null);

            int wrongDoorEvents = 0;
            System.Action<PlayerBehaviorEvent> handler = behavior =>
            {
                if (behavior == PlayerBehaviorEvent.WrongDoor) wrongDoorEvents++;
            };

            tracker.BehaviorDetected += handler;
            encounters.RegisterWrongDoorAttempt();
            tracker.BehaviorDetected -= handler;

            Assert.That(wrongDoorEvents, Is.EqualTo(1), "Stage 3 must preserve the Stage 2 WrongDoor signal.");
        }
    }
}
