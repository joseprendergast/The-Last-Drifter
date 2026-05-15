# The Last Drifter - Unity Production Project

This is the production Unity build for **The Last Drifter**. The browser prototype at the repository root is the mood, story, and interaction reference. This Unity project is the real game foundation.

## Chosen Stack

- Unity 6.3 LTS or newer Unity 6 LTS
- Universal Render Pipeline with 2D Renderer
- Adventure Creator for point-and-click adventure systems
- Unity Input System
- Pixel Perfect Camera
- TextMeshPro

Adventure Creator is a paid Unity Asset Store package, so it is not committed here. Import it through Package Manager / My Assets after opening the project.

## First Vertical Slice

1. Main menu
2. Alley scene
3. Contextual hotspots
4. Case file / evidence collection
5. Subtitle presentation
6. Save/load
7. Rain, flicker, and noir post-processing
8. Transition into the Blood Lab placeholder

## Open The Project

1. Install Unity Hub.
2. Install Unity 6.3 LTS or the latest Unity 6 LTS.
3. In Unity Hub, choose **Add project from disk**.
4. Select `Unity/TheLastDrifter`.
5. Let Unity restore packages.
6. Import Adventure Creator from the Unity Asset Store.
7. Set the project scripting define symbol `TLD_ADVENTURE_CREATOR` only after AC is imported and the bridge is wired.

## Production Rule

The HTML prototype is not converted directly. It is used as the narrative blueprint. Unity owns the final scene logic, art, audio, save files, and builds.

