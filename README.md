# WE'RE MAKING FUN OF YOU

A first-person horror-comedy game about a game show that knows you're playing.

A Unity 3D project targeting Windows and macOS, with English and Turkish localization from the beginning.

## Current development stage

Stage 2: Host character and reactive dialogue foundation

- Runtime-generated prototype studio environment
- First-person movement, mouse look, and sprint
- Raycast interaction using E
- Visible host character with a simple procedural 3D body
- Host turns to face the player
- Player behavior signals for idling, sustained sprinting, wrong-door attempts, and abandoned interactions
- Bilingual reactive host dialogue with event priorities and cooldowns
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
- E: Interact
- Escape: Release mouse cursor

## Input handling note

The current controller uses Unity's legacy Input Manager API (Input.GetAxis, Input.GetAxisRaw, and Input.GetKey). When opening the project in Unity, check **Edit > Project Settings > Player > Other Settings > Configuration > Active Input Handling** and select **Input Manager (Old)** or **Both**. The repository currently does not contain a committed ProjectSettings/ProjectSettings.asset, so this setting has not been pinned in source control and must be verified in the Editor.

## Localization

The first screen offers ENGLISH and TÜRKÇE. Runtime text is routed through LocalizedText, so dialogue and objective text can be expanded without scattering language checks throughout gameplay code.

## Verification

Run the repository integrity check:

    python3 Tools/validate_project.py

The GitHub Actions check validates project files and bilingual localization keys. It is not a Unity Editor compile or a Windows/macOS player build. Those require the Unity Editor and will be verified separately.

## Project identity

- Title: WE'RE MAKING FUN OF YOU
- Genre: First-person horror comedy
- Target playtime: approximately 60 minutes for the full game
- Target platforms: Windows and macOS
- Languages: English and Turkish
