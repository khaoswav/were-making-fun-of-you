using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class HostCharacterController : MonoBehaviour
    {
        [SerializeField] private float turnSpeed = 3.5f;
        [SerializeField] private float lookHeight = 1.45f;

        private Transform player;
        private Vector3 basePosition;
        private float idlePhase;

        private void Start()
        {
            basePosition = transform.position;
            var camera = Camera.main;
            if (camera != null) player = camera.transform;
        }

        private void Update()
        {
            if (player == null)
            {
                var camera = Camera.main;
                if (camera != null) player = camera.transform;
                if (player == null) return;
            }

            Vector3 direction = player.position - (transform.position + Vector3.up * lookHeight);
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            idlePhase += Time.deltaTime;
            transform.position = basePosition + Vector3.up * (Mathf.Sin(idlePhase * 1.4f) * 0.025f);
        }
    }
}
