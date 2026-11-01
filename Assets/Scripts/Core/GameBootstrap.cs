using UnityEngine;

namespace WereMakingFunOfYou
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntimeGame()
        {
            if (Object.FindFirstObjectByType<GameDirector>() != null) return;
            var root = new GameObject("WMFO_Runtime");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<GameDirector>();
        }
    }
}
