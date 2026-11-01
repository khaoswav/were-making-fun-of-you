from pathlib import Path
import json
import sys

root = Path(__file__).resolve().parents[1]
required = [
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/Scripts/WereMakingFunOfYou.Runtime.asmdef",
    "Assets/Editor/WereMakingFunOfYou.Editor.asmdef",
    "Assets/Tests/Editor/WereMakingFunOfYou.EditorTests.asmdef",
    "Assets/Tests/Editor/LegacyInputHandlingSetupTests.cs",
    "Assets/Tests/Editor/LocalizationTests.cs",
    "Assets/Tests/PlayMode/WereMakingFunOfYou.PlayModeTests.asmdef",
    "Assets/Tests/PlayMode/GameRuntimeSmokeTests.cs",
    "Assets/Tests/PlayMode/MonsterChaseControllerPlayModeTests.cs",
    "Assets/Scripts/Core/GameBootstrap.cs",
    "Assets/Scripts/Core/GameDirector.cs",
    "Assets/Scripts/Player/FirstPersonController.cs",
    "Assets/Scripts/Player/PlayerBehaviorTracker.cs",
    "Assets/Scripts/Player/InteractionRaycaster.cs",
    "Assets/Scripts/World/WorldInteractable.cs",
    "Assets/Scripts/World/HostCharacterController.cs",
    "Assets/Scripts/World/ReactiveDialogueController.cs",
    "Assets/Scripts/World/HorrorEncounterDirector.cs",
    "Assets/Scripts/World/MonsterChaseController.cs",
    "Assets/Scripts/Localization/GameLanguage.cs",
    "Assets/Scripts/Localization/LocalizedText.cs",
    "Assets/Editor/LegacyInputHandlingSetup.cs",
]
errors = []

for relative in required:
    if not (root / relative).is_file():
        errors.append(f"Missing required file: {relative}")

try:
    manifest = json.loads((root / "Packages/manifest.json").read_text())
    if manifest.get("dependencies", {}).get("com.unity.test-framework") != "1.4.5":
        errors.append("Unity Test Framework dependency must be pinned to 1.4.5.")
except Exception as exc:
    errors.append(f"Invalid package manifest: {exc}")

assemblies = {
    "Assets/Scripts/WereMakingFunOfYou.Runtime.asmdef": "WereMakingFunOfYou.Runtime",
    "Assets/Editor/WereMakingFunOfYou.Editor.asmdef": "WereMakingFunOfYou.Editor",
    "Assets/Tests/Editor/WereMakingFunOfYou.EditorTests.asmdef": "WereMakingFunOfYou.EditorTests",
    "Assets/Tests/PlayMode/WereMakingFunOfYou.PlayModeTests.asmdef": "WereMakingFunOfYou.PlayModeTests",
}
parsed_assemblies = {}
for relative, expected_name in assemblies.items():
    try:
        definition = json.loads((root / relative).read_text())
        parsed_assemblies[relative] = definition
        if definition.get("name") != expected_name:
            errors.append(f"Unexpected assembly name in {relative}")
        if "Tests" in expected_name and "TestAssemblies" not in definition.get("optionalUnityReferences", []):
            errors.append(f"Test assembly is missing TestAssemblies reference: {relative}")
    except Exception as exc:
        errors.append(f"Invalid assembly definition {relative}: {exc}")

runtime_assembly = parsed_assemblies.get("Assets/Scripts/WereMakingFunOfYou.Runtime.asmdef", {})
if "UnityEngine.UI" not in runtime_assembly.get("references", []):
    errors.append("Runtime assembly must reference UnityEngine.UI for uGUI/EventSystem.")

editor_tests_assembly = parsed_assemblies.get("Assets/Tests/Editor/WereMakingFunOfYou.EditorTests.asmdef", {})
if "Editor" not in editor_tests_assembly.get("includePlatforms", []):
    errors.append("Editor tests must be restricted to the Editor platform.")

manifest_version = (root / "ProjectSettings/ProjectVersion.txt").read_text()
if "6000.3.0f1" not in manifest_version:
    errors.append("Project Editor version does not match Unity 6000.3.0f1.")

localization = (root / "Assets/Scripts/Localization/LocalizedText.cs").read_text()
phrases = (
    "objective_intro",
    "host_intro",
    "interaction_hint",
    "door_locked",
    "generic_interact",
    "host_wrong_door",
    "host_wrong_door_alt",
    "host_idle",
    "host_idle_alt",
    "host_sprinting",
    "host_sprinting_alt",
    "host_avoids_interaction",
    "host_avoids_interaction_alt",
    "host_wrong_door_3",
    "host_wrong_door_4",
    "host_locker_hide_prompt",
    "host_locker_peek",
    "host_locker_exit",
    "host_door_wave",
    "host_fake_jumpscare",
    "host_fake_jumpscare_punchline",
    "host_fake_scare_repeat",
    "host_safe_room_welcome",
    "host_safe_room_door_closed",
    "host_safe_room_trap",
    "host_monster_enraged",
    "host_real_jumpscare",
)
for phrase in phrases:
    if localization.count(f'["{phrase}"]') != 2:
        errors.append(f"Localization key must exist in both languages: {phrase}")

input_setup = (root / "Assets/Editor/LegacyInputHandlingSetup.cs").read_text()
for marker in ("activeInputHandler", "property.intValue = 0;", "Enable Legacy Input Manager", "TryGetActiveInputHandler"):
    if marker not in input_setup:
        errors.append(f"Missing legacy input configuration marker: {marker}")

encounters = (root / "Assets/Scripts/World/HorrorEncounterDirector.cs").read_text()
for marker in (
    "locker_hide",
    "prank_door",
    "fake_scare",
    "safe_room_door",
    "RegisterWrongDoorAttempt",
    "RealScareSequence",
    "behaviorTracker?.ReportWrongDoor();",
    "hostMotion?.WalkTo(hostStagePosition);",
):
    if marker not in encounters:
        errors.append(f"Missing encounter implementation marker: {marker}")

test_markers = {
    "Assets/Tests/Editor/LegacyInputHandlingSetupTests.cs": ("ActiveInputHandlerCanBeReadWithoutChangingIt",),
    "Assets/Tests/Editor/LocalizationTests.cs": ("EveryRequiredKeyResolvesToLocalizedText",),
    "Assets/Tests/PlayMode/GameRuntimeSmokeTests.cs": (
        "RuntimeBootstrapCreatesExactlyOneWorld",
        "WrongDoorAttemptStillRaisesStageTwoBehaviorEvent",
    ),
    "Assets/Tests/PlayMode/MonsterChaseControllerPlayModeTests.cs": (
        "CatchCallbackFiresWhenTargetEntersCatchRadius",
        "StopChasePreventsFurtherMovement",
        "ChaseStumblesThenRaisesEnragedCallbackAndMovesFaster",
    ),
}
for relative, markers in test_markers.items():
    source = (root / relative).read_text()
    for marker in markers:
        if marker not in source:
            errors.append(f"Missing Unity test scenario {marker} in {relative}")

for path in (root / "Assets").rglob("*.cs"):
    source = path.read_text()
    if source.count("{") != source.count("}"):
        errors.append(f"Unbalanced braces: {path.relative_to(root)}")

if errors:
    print("\n".join(errors))
    sys.exit(1)

print(
    f"Project integrity passed: {len(required)} required files, pinned Unity Test Framework, "
    f"{len(assemblies)} assembly definitions, {len(phrases)} bilingual keys, "
    "encounter regression hooks, Unity test scenarios, and C# brace checks."
)
