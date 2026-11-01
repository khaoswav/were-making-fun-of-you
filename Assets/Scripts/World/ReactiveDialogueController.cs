using System.Collections.Generic;
using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class ReactiveDialogueController : MonoBehaviour
    {
        private sealed class PendingLine
        {
            public string Key;
            public int Priority;
            public float QueuedAt;
        }

        [SerializeField] private float globalCooldown = 2.8f;
        [SerializeField] private float queueLifetime = 12f;

        private readonly List<PendingLine> pending = new List<PendingLine>();
        private readonly Dictionary<PlayerBehaviorEvent, float> nextAllowedAt =
            new Dictionary<PlayerBehaviorEvent, float>();
        private readonly Dictionary<PlayerBehaviorEvent, int> nextVariantIndex =
            new Dictionary<PlayerBehaviorEvent, int>();
        private PlayerBehaviorTracker tracker;
        private float nextDialogueAt;
        private bool subscribed;

        private void Update()
        {
            if (!subscribed && GameDirector.Instance != null && GameDirector.Instance.IsStarted)
                SubscribeToPlayer();

            for (int i = pending.Count - 1; i >= 0; i--)
                if (Time.time - pending[i].QueuedAt > queueLifetime) pending.RemoveAt(i);

            if (pending.Count == 0 || Time.time < nextDialogueAt) return;

            pending.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            PendingLine line = pending[0];
            pending.RemoveAt(0);

            if (GameDirector.Instance != null)
                GameDirector.Instance.ShowHostLine(line.Key, 4.5f);

            nextDialogueAt = Time.time + globalCooldown;
        }

        private void SubscribeToPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            tracker = player.GetComponent<PlayerBehaviorTracker>();
            if (tracker == null) return;

            tracker.BehaviorDetected += OnBehaviorDetected;
            subscribed = true;
        }

        private void OnDestroy()
        {
            if (tracker != null) tracker.BehaviorDetected -= OnBehaviorDetected;
        }

        private void OnBehaviorDetected(PlayerBehaviorEvent behavior)
        {
            if (nextAllowedAt.TryGetValue(behavior, out float allowedAt) && Time.time < allowedAt)
                return;

            string key;
            int priority;
            float cooldown;

            switch (behavior)
            {
                case PlayerBehaviorEvent.WrongDoor:
                    key = "host_wrong_door"; priority = 100; cooldown = 10f; break;
                case PlayerBehaviorEvent.AvoidedInteraction:
                    key = "host_avoids_interaction"; priority = 75; cooldown = 18f; break;
                case PlayerBehaviorEvent.Sprinting:
                    key = "host_sprinting"; priority = 55; cooldown = 22f; break;
                default:
                    key = "host_idle"; priority = 35; cooldown = 30f; break;
            }

            nextAllowedAt[behavior] = Time.time + cooldown;

            for (int i = 0; i < pending.Count; i++)
                if (pending[i].Key.StartsWith(key)) return;

            int variant = nextVariantIndex.TryGetValue(behavior, out int next) ? next : 0;
            nextVariantIndex[behavior] = (variant + 1) % 2;
            if (variant == 1) key += "_alt";

            pending.Add(new PendingLine { Key = key, Priority = priority, QueuedAt = Time.time });
        }
    }
}
