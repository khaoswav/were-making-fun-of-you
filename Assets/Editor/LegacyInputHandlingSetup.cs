#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEditor.Build.Profile;
#endif

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
            SerializedObject serializedSettings;
            PlayerSettings settings;
            SerializedProperty property = FindInputHandlingProperty(out serializedSettings, out settings);
            if (property == null)
            {
                EditorUtility.DisplayDialog(
                    "Input settings unavailable",
                    "Unity Player Settings could not be located. Open Project Settings > Player and set Active Input Handling to Input Manager (Old) or Both.",
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
            SerializedObject serializedSettings;
            PlayerSettings settings;
            SerializedProperty property = FindInputHandlingProperty(out serializedSettings, out settings);
            if (property == null)
            {
                Debug.LogWarning(
                    "[WE'RE MAKING FUN OF YOU] Could not read Active Input Handling. Verify Project Settings > Player before entering Play Mode.");
                return;
            }

            if (property.intValue == 1)
            {
                Debug.LogWarning(
                    "[WE'RE MAKING FUN OF YOU] The active Player Settings use Input System (New), but the controller and uGUI use UnityEngine.Input / StandaloneInputModule. Choose Tools > WE'RE MAKING FUN OF YOU > Enable Legacy Input Manager, or manually select Both.");
            }
        }

        private static SerializedProperty FindInputHandlingProperty(
            out SerializedObject serializedSettings,
            out PlayerSettings settings)
        {
            serializedSettings = null;
            settings = null;

#if UNITY_6000_0_OR_NEWER
            Type playerSettingsType = typeof(BuildProfile);
            FieldInfo globalSettingsField = playerSettingsType.GetField(
                "s_GlobalPlayerSettings",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (globalSettingsField == null)
            {
                Debug.LogWarning("[WE'RE MAKING FUN OF YOU] Unity's global Player Settings field was not found. Update the Editor utility for this Unity version.");
                return null;
            }

            settings = globalSettingsField.GetValue(null) as PlayerSettings;
            BuildProfile activeProfile = BuildProfile.GetActiveBuildProfile();
            if (activeProfile != null)
            {
                FieldInfo overrideField = playerSettingsType.GetField(
                    "m_PlayerSettings",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (overrideField != null)
                {
                    PlayerSettings profileSettings = overrideField.GetValue(activeProfile) as PlayerSettings;
                    if (profileSettings != null) settings = profileSettings;
                }
            }
#else
            settings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
#endif

            if (settings == null) return null;
            serializedSettings = new SerializedObject(settings);
            return serializedSettings.FindProperty(InputHandlingProperty);
        }
    }
}
#endif
