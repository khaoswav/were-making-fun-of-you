using System;
using UnityEngine;

namespace WereMakingFunOfYou
{
    public enum PlayerBehaviorEvent
    {
        Idle,
        Sprinting,
        WrongDoor,
        AvoidedInteraction
    }

    public sealed class PlayerBehaviorTracker : MonoBehaviour
    {
        [SerializeField] private float idleThreshold = 8f;
        [SerializeField] private float sprintThreshold = 2.5f;

        public event Action<PlayerBehaviorEvent> BehaviorDetected;

        private float lastMovementTime;
        private float sprintStartedAt = -1f;
        private bool idleReported;
        private bool sprintReported;
        private bool wasLookingAtInteractable;
        private bool interactedDuringLook;

        private void Awake()
        {
            lastMovementTime = Time.time;
        }

        public void ReportMovement(Vector2 input, bool sprinting)
        {
            bool moving = input.sqrMagnitude > 0.01f;
            if (moving)
            {
                lastMovementTime = Time.time;
                idleReported = false;
            }

            if (sprinting && moving)
            {
                if (sprintStartedAt < 0f) sprintStartedAt = Time.time;
                if (!sprintReported && Time.time - sprintStartedAt >= sprintThreshold)
                {
                    sprintReported = true;
                    BehaviorDetected?.Invoke(PlayerBehaviorEvent.Sprinting);
                }
            }
            else
            {
                sprintStartedAt = -1f;
                sprintReported = false;
            }

            if (!idleReported && Time.time - lastMovementTime >= idleThreshold)
            {
                idleReported = true;
                BehaviorDetected?.Invoke(PlayerBehaviorEvent.Idle);
            }
        }

        public void ObserveInteractable(bool visible)
        {
            if (visible)
            {
                if (!wasLookingAtInteractable)
                    interactedDuringLook = false;
                wasLookingAtInteractable = true;
                return;
            }

            if (wasLookingAtInteractable)
            {
                wasLookingAtInteractable = false;
                if (!interactedDuringLook)
                    BehaviorDetected?.Invoke(PlayerBehaviorEvent.AvoidedInteraction);
            }
        }

        public void ReportInteraction()
        {
            if (wasLookingAtInteractable) interactedDuringLook = true;
            lastMovementTime = Time.time;
            idleReported = false;
        }

        public void ReportWrongDoor()
        {
            BehaviorDetected?.Invoke(PlayerBehaviorEvent.WrongDoor);
        }
    }
}
