#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WereMakingFunOfYou.Editor
{
    [InitializeOnLoad]
    public static class LegacyInputHandlingSetup
    {
        private const string InputHandlingProperty = "activeInputHandler";

        static LegacyInputHandlingSetup()
        {
            EditorApplication.delayCall += ValidateInputHandling;
        }

        [MenuItem("Tools/WE'RE MAKING FUN OF YOU/Enable Legacy Input Manager")]
        private static void EnableLegacyInputManager()
        {
            PlayerSettings settings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "Input settings unavailable",
                    "Unity Player Settings could not be located. Open Project Settings > Player and set Active Input Handling to Input Manager (Old) or Both.",
                    "OK");
                return;
            }

            SerializedObject serializedSettings = new SerializedObject(settings);
            SerializedProperty property = serializedSettings.FindProperty(InputHandlingProperty);
            if (property == null)
            {
                EditorUtility.DisplayDialog(
                    "Input setting unavailable",
                    "This Unity version does not expose the expected Active Input Handling setting. Set it manually to Input Manager (Old) or Both.",
                    "OK");
                return;
            }

            Undo.RecordObject(settings, "Enable Legacy Input Manager");
            property.intValue = 0;
            serializedSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "Legacy Input Manager enabled",
                "Active Input Handling is now set to Input Manager (Old). Restart the Unity Editor if prompted, then enter Play Mode.",
                "OK");
        }

        private static void ValidateInputHandling()
        {
            PlayerSettings settings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
            if (settings == null) return;

            SerializedObject serializedSettings = new SerializedObject(settings);
            SerializedProperty property = serializedSettings.FindProperty(InputHandlingProperty);
            if (property == null)
            {
                Debug.LogWarning(
                    "[WE'RE MAKING FUN OF YOU] Could not read Active Input Handling. Verify Project Settings > Player before entering Play Mode.");
                return;
            }

            if (property.intValue == 1)
            {
                Debug.LogWarning(
                    "[WE'RE MAKING FUN OF YOU] The project currently uses Input System (New), but the controller and UI use UnityEngine.Input / StandaloneInputModule. Choose Tools > WE'RE MAKING FUN OF YOU > Enable Legacy Input Manager, or manually select Both.");
            }
        }
    }
}
#endif