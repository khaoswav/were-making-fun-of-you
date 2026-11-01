from pathlib import Path
import json
import sys

root = Path(__file__).resolve().parents[1]
required = [
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
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
    json.loads((root / "Packages/manifest.json").read_text())
except Exception as exc:
    errors.append(f"Invalid package manifest: {exc}")

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
for marker in ("activeInputHandler", "property.intValue = 0;", "Enable Legacy Input Manager"):
    if marker not in input_setup:
        errors.append(f"Missing legacy input configuration marker: {marker}")

encounters = (root / "Assets/Scripts/World/HorrorEncounterDirector.cs").read_text()
for marker in ("locker_hide", "prank_door", "fake_scare", "safe_room_door", "RegisterWrongDoorAttempt", "RealScareSequence"):
    if marker not in encounters:
        errors.append(f"Missing encounter implementation marker: {marker}")

for path in (root / "Assets").rglob("*.cs"):
    source = path.read_text()
    if source.count("{") != source.count("}"):
        errors.append(f"Unbalanced braces: {path.relative_to(root)}")

if errors:
    print("\n".join(errors))
    sys.exit(1)

print(f"Project integrity passed: {len(required)} required files, package manifest, {len(phrases)} bilingual keys, legacy input setup, encounter hooks, and C# brace checks.")
