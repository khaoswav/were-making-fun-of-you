using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class HostCharacterController : MonoBehaviour
    {
        [SerializeField] private float turnSpeed = 3.5f;
        [SerializeField] private float lookHeight = 1.45f;
        [SerializeField] private float walkSpeed = 2.5f;

        private Transform player;
        private Transform rightArm;
        private Vector3 basePosition;
        private Vector3 walkDestination;
        private Quaternion restingArmRotation;
        private float idlePhase;
        private float waveUntil;
        private bool walking;

        private void Start()
        {
            basePosition = transform.position;
            rightArm = transform.Find("Right Arm");
            if (rightArm != null) restingArmRotation = rightArm.localRotation;
            var camera = Camera.main;
            if (camera != null) player = camera.transform;
        }

        public void WalkTo(Vector3 destination)
        {
            walkDestination = destination;
            walking = true;
        }

        public void TriggerWave(float duration)
        {
            waveUntil = Time.time + duration;
        }

        private void Update()
        {
            if (player == null)
            {
                var camera = Camera.main;
                if (camera != null) player = camera.transform;
            }

            if (walking)
            {
                basePosition = Vector3.MoveTowards(basePosition, walkDestination, walkSpeed * Time.deltaTime);
                if ((basePosition - walkDestination).sqrMagnitude < 0.01f) walking = false;
            }

            if (player != null)
            {
                Vector3 direction = player.position - (transform.position + Vector3.up * lookHeight);
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion target = Quaternion.LookRotation(direction.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
                }
            }

            idlePhase += Time.deltaTime;
            transform.position = basePosition + Vector3.up * (Mathf.Sin(idlePhase * 1.4f) * 0.025f);

            if (rightArm != null)
            {
                float wave = Time.time < waveUntil ? Mathf.Sin(Time.time * 13f) * 48f : 0f;
                rightArm.localRotation = restingArmRotation * Quaternion.Euler(0f, 0f, wave);
            }
        }
    }
}