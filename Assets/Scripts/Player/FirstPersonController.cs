using UnityEngine;

namespace WereMakingFunOfYou
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4.2f;
        [SerializeField] private float sprintSpeed = 6.8f;
        [SerializeField] private float mouseSensitivity = 2.1f;
        [SerializeField] private float gravity = -20f;
        private CharacterController character;
        private Transform playerCamera;
        private float verticalVelocity;
        private float pitch;

        private void Awake()
        {
            character = GetComponent<CharacterController>();
            playerCamera = GetComponentInChildren<Camera>().transform;
        }

        private void Update()
        {
            if (GameDirector.Instance == null || !GameDirector.Instance.IsStarted) return;
            Look();
            Move();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Look()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch - mouseY, -82f, 82f);
            playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void Move()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 direction = (transform.right * x + transform.forward * z).normalized;
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            if (character.isGrounded && verticalVelocity < 0) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = direction * speed;
            velocity.y = verticalVelocity;
            character.Move(velocity * Time.deltaTime);
        }
    }
}
