# WE'RE MAKING FUN OF YOU

A first-person horror-comedy game about a game show that knows you're playing.

A Unity 3D project targeting Windows and macOS, with English and Turkish localization from the beginning.

## Current development stage

Stage 3: Interactive horror-comedy encounters

- Runtime-generated studio, host character, and player controller
- First-person movement, mouse look, sprint, and E interaction
- Host turns toward the player, walks to encounter locations, and waves
- Player behavior signals and bilingual reactive host dialogue
- Hide inside a locker while the host walks over, opens it, and stares at you
- A door closes, reopens, and the host waves through it
- Fake jumpscare button with a face pop-in and a punchline
- Escalating wrong-door reactions
- A safe room that becomes a trap and launches a monster chase
- The monster stumbles, gets back up, and becomes faster and angrier
- English / Turkish language selection at startup
- Source-integrity GitHub Actions workflow

## Requirements

- Unity 6000.3.0f1
- Windows or macOS editor
- No paid Unity packages are required for the current prototype

## Open the project

1. Clone or download this repository.
2. In Unity Hub, choose Add > Add project from disk.
3. Select the repository folder.
4. Open it with Unity 6000.3.0f1.
5. Open any scene and press Play. The prototype environment is generated at runtime.

## Controls

- W A S D: Move
- Mouse: Look
- Left Shift: Sprint
- E: Interact, enter or leave the locker
- Escape: Release mouse cursor

## Input handling

The current first-person controller and uGUI EventSystem use Unity's legacy Input Manager APIs. On first open, the Editor checks the Active Input Handling setting and warns if only the new Input System is enabled.

To configure the project from the Editor, choose **Tools > WE'RE MAKING FUN OF YOU > Enable Legacy Input Manager**. This sets Active Input Handling to Input Manager (Old). Restart the Editor if Unity prompts you. You can alternatively select Both under **Edit > Project Settings > Player > Other Settings > Configuration > Active Input Handling**.

The repository still does not include a committed ProjectSettings/ProjectSettings.asset or a hand-authored Unity scene. The setting tool applies the change inside the Unity Editor rather than shipping an incomplete PlayerSettings asset.

## Localization

The first screen offers ENGLISH and TÜRKÇE. Runtime text is routed through LocalizedText, including all encounter reactions.

## Verification

Run the repository integrity check:

    python3 Tools/validate_project.py

GitHub Actions validates project structure, bilingual dialogue keys, encounter hooks, and the Editor input setup helper. It does not compile C# against Unity assemblies, run Play Mode, or create Windows/macOS player builds. Those checks still require Unity 6000.3.0f1 and must be performed in the Editor.

## Project identity

- Title: WE'RE MAKING FUN OF YOU
- Genre: First-person horror comedy
- Target playtime: approximately 60 minutes for the full game
- Target platforms: Windows and macOS
- Languages: English and Turkish
