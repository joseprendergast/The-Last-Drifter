# The Last Drifter

The Last Drifter is a noir point-and-click adventure about Captain Gabardina and a city where memories are becoming physical.

The public GitHub Pages build now lives in `docs/` and is exported from the Unity + PowerQuest production project. The original root `index.html` browser prototype remains in the repo as a fast story, mood, and scene reference.

## Play

Open the published build at:

`https://joseprendergast.github.io/The-Last-Drifter/`

For quick reference work, the old HTML prototype can still be opened from `index.html`.

## Current Playable Slice

- Title screen with Start Game, Continue, Scene Select, Settings, and Reset Progress.
- Three playable scenes: The Alley, The Blood Lab, and The Zoo.
- Contextual canvas hotspots for investigation instead of a visible verb bar.
- Case file and evidence collection.
- Persistent progress with localStorage.
- Scene completion, objective tracking, debug flag inspection, and reset.
- Optional generated ambience/audio after the player starts it.

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
