using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class InteractionRaycaster : MonoBehaviour
    {
        [SerializeField] private float distance = 3f;
        private Camera viewCamera;
        private WorldInteractable current;
        private PlayerBehaviorTracker behaviorTracker;

        private void Awake()
        {
            viewCamera = GetComponentInChildren<Camera>();
            behaviorTracker = GetComponent<PlayerBehaviorTracker>();
        }

        private void Update()
        {
            if (GameDirector.Instance == null || !GameDirector.Instance.IsStarted) return;
            current = null;
            if (Physics.Raycast(viewCamera.transform.position, viewCamera.transform.forward, out RaycastHit hit, distance))
                current = hit.collider.GetComponent<WorldInteractable>();

            if (behaviorTracker != null) behaviorTracker.ObserveInteractable(current != null);
            GameDirector.Instance.ShowPrompt(current != null, "interaction_hint");

            if (current != null && Input.GetKeyDown(KeyCode.E))
            {
                if (behaviorTracker != null) behaviorTracker.ReportInteraction();
                current.Interact();
            }
        }
    }
}
