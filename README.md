# The Last Drifter

The Last Drifter is a noir point-and-click adventure about Captain Gabardina and a city where memories are becoming physical.

The public GitHub Pages build now lives in `docs/` and is exported from the Unity + PowerQuest production project. The original browser prototype remains in the repo as `prototype.html` for fast story, mood, and scene reference.

## Play

Open the published build at:

`https://joseprendergast.github.io/The-Last-Drifter/`

For quick reference work, the old HTML prototype can still be opened from `prototype.html`.

## Current Playable Slice

- Unity + PowerQuest production slice exported to `docs/` for GitHub Pages.
- Cinematic noir title overlay with rain, scanlines, a dark city background, and a minimal menu.
- First playable Alley scene with Captain Gabardina, rain, severed-hand evidence, drain clue, and service-door objective.
- Contextual interaction model: left click acts, right click inspects, compact dock selects inspect/move/action/case modes.
- Compact hidden case-file strip instead of a permanent inventory bar.
- Dark cinematic subtitle treatment replacing the default bright PowerQuest display box.
- Animated rain and scanline presentation overlays for the menu and first scene.

## Scene Flow

1. **The Alley**: Gabardina wakes in heavy rain with a severed hand on his chest, then follows the blood toward a service door.
2. **The Blood Lab**: An accountant enters a sealed lavatory/lab, restores power, reads the monitors, and recovers a memory reel.
3. **The Zoo**: A normal morning breaks open when the animals scream, the sign bleeds, and a child explains what is happening.

## Publishing With GitHub Pages

1. Open the repository on GitHub.
2. Go to **Settings**.
3. Open **Pages**.
4. Set the source to the default branch and the `/docs` folder.
5. Save. GitHub Pages will serve the Unity WebGL build from `docs/index.html`.

The expected project-page URL is:

`https://joseprendergast.github.io/The-Last-Drifter/`

## Next Steps

- Continue production in `Unity/TheLastDrifter`.
- Use PowerQuest as the Unity point-and-click framework.
- Treat the HTML game as the story, mood, and scene reference while PowerQuest becomes the production build.
- Build the Alley vertical slice first, then split the Blood Lab / Lavatory and Zoo into proper PowerQuest rooms.
- Replace the imported sample art with bespoke noir character, prop, and room sprites.

## Noir UI Direction

- Palette anchor: near-black, deep navy, wet asphalt, muted slate, cold cyan, sodium amber, and dried-blood red.
- UI should stay minimal: title/menu overlays, compact bottom dock, hidden case file, and short subtitle blocks.
- Keep default PowerQuest UI hidden unless it is restyled first.
- Controls: left click performs the contextual action, right click inspects, the case button opens evidence, and move/look/action are selected from the compact dock.
- The UI layer lives primarily in `Assets/Game/TheLastDrifter/DrifterActionToolbar.cs`; room narrative remains in the PowerQuest room scripts.
