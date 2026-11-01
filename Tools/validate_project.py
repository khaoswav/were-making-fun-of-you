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
    "Assets/Scripts/Localization/GameLanguage.cs",
    "Assets/Scripts/Localization/LocalizedText.cs",
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
    "host_idle",
    "host_sprinting",
    "host_avoids_interaction",
)
for phrase in phrases:
    if localization.count(f'["{phrase}"]') != 2:
        errors.append(f"Localization key must exist in both languages: {phrase}")

for path in (root / "Assets").rglob("*.cs"):
    source = path.read_text()
    if source.count("{") != source.count("}"):
        errors.append(f"Unbalanced braces: {path.relative_to(root)}")

if errors:
    print("\n".join(errors))
    sys.exit(1)

print(f"Project integrity passed: {len(required)} required files, package manifest, {len(phrases)} bilingual keys, and C# brace checks.")
