# Unity Production Plan

## Stack

Use Unity 6.3 LTS, URP 2D Renderer, Adventure Creator, Pixel Perfect Camera, TextMeshPro, and the Input System.

The HTML build stays as the interactive story reference. Unity is the production game.

## Vertical Slice

The first shippable slice is **The Alley**:

- Main menu
- Noir title card
- Rainy alley with layered pixel background
- Captain Gabardina wake-up beat
- Hotspots: hand, coat, drain, service door
- Evidence: severed hand, gabardine threads, blood in drain
- Case file menu
- Save/load
- Transition to Blood Lab placeholder

## Adventure Creator Setup

After importing Adventure Creator:

- Use AC Hotspots for player interactions.
- Use AC Inventory for evidence-like items only when they are truly usable.
- Keep the custom Case File system for detective evidence and lore.
- Use AC ActionLists for scene transitions and cutscenes.
- Use AC Variables for simple scene gating, or call `AdventureCreatorBridge` methods for custom flags.

## Visual Standards

- 320x180 or 480x270 internal composition, scaled through Pixel Perfect Camera.
- URP 2D lights for lamps, alarm flashes, and sign glows.
- Film grain, bloom, color grading, and subtle chromatic aberration through URP post-processing.
- No clean UI panels in scene unless they are part of the case file or menu.

## Art Passes

1. Blockout with placeholder pixel shapes.
2. Paint background layers.
3. Add foreground silhouettes and interactable props.
4. Add lighting and normal-map tests.
5. Add rain, reflections, and screen-space grime.
6. Add final animation and audio.

## Engineering Milestones

1. Project opens and compiles.
2. Main menu loads Alley scene.
3. Alley hotspots trigger flags/evidence/subtitles.
4. Save file survives restart.
5. Case file renders evidence.
6. Adventure Creator imported and configured.
7. Desktop build produced.

