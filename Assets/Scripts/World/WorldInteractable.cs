using UnityEngine;

namespace WereMakingFunOfYou
{
    public sealed class WorldInteractable : MonoBehaviour
    {
        [SerializeField] private string contentKey = "generic";
        public void SetContent(string key) => contentKey = key;
        public void Interact()
        {
            if (GameDirector.Instance != null) GameDirector.Instance.Interact(contentKey);
        }
    }
}
