using System;
using UnityEngine;

namespace WereMakingFunOfYou
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class MonsterChaseController : MonoBehaviour
    {
        private enum ChasePhase
        {
            Dormant,
            Chasing,
            Stumbling,
            Enraged
        }

        [SerializeField] private float chaseSpeed = 3.8f;
        [SerializeField] private float enragedSpeed = 6.6f;
        [SerializeField] private float catchDistance = 1.35f;
        [SerializeField] private float stumbleAfter = 5.5f;
        [SerializeField] private float stumbleDuration = 1.25f;

        private CharacterController character;
        private Transform visualRoot;
        private Transform target;
        private Action caughtCallback;
        private Action enragedCallback;
        private ChasePhase phase;
        private float chaseStartedAt;
        private float stumbleEndsAt;
        private Quaternion uprightRotation;

        private void Awake()
        {
            character = GetComponent<CharacterController>();
            visualRoot = transform.Find("Monster Visuals");
            if (visualRoot != null) uprightRotation = visualRoot.localRotation;
        }

        public void BeginChase(Transform chaseTarget, Action onCaught, Action onEnraged)
        {
            if (chaseTarget == null) return;

            target = chaseTarget;
            caughtCallback = onCaught;
            enragedCallback = onEnraged;
            chaseStartedAt = Time.time;
            phase = ChasePhase.Chasing;
            if (visualRoot != null) visualRoot.localRotation = uprightRotation;
        }

        public void StopChase()
        {
            phase = ChasePhase.Dormant;
            target = null;
            caughtCallback = null;
            enragedCallback = null;
            if (visualRoot != null) visualRoot.localRotation = uprightRotation;
        }

        private void Update()
        {
            if (target == null || phase == ChasePhase.Dormant) return;

            if (phase == ChasePhase.Stumbling)
            {
                if (Time.time >= stumbleEndsAt)
                {
                    phase = ChasePhase.Enraged;
                    if (visualRoot != null) visualRoot.localRotation = uprightRotation;
                    enragedCallback?.Invoke();
                }
                return;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            float distance = direction.magnitude;

            if (distance <= catchDistance)
            {
                Action callback = caughtCallback;
                StopChase();
                callback?.Invoke();
                return;
            }

            if (phase == ChasePhase.Chasing && Time.time - chaseStartedAt >= stumbleAfter)
            {
                phase = ChasePhase.Stumbling;
                stumbleEndsAt = Time.time + stumbleDuration;
                if (visualRoot != null)
                    visualRoot.localRotation = uprightRotation * Quaternion.Euler(0f, 0f, 78f);
                return;
            }

            if (direction.sqrMagnitude < 0.001f) return;

            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 7f);
            float speed = phase == ChasePhase.Enraged ? enragedSpeed : chaseSpeed;
            character.Move(direction.normalized * speed * Time.deltaTime);
        }
    }
}