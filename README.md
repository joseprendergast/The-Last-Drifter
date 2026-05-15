# The Last Drifter

The Last Drifter is a static, browser-playable noir pixel adventure about Captain Gabardina and a city where memories are becoming physical.

`index.html` is the public GitHub Pages entry point and the current source of truth for the playable prototype. It preserves the original cinematic rain, title card, canvas rendering, subtitles, and three core scenes, then adds the first adventure-game layer on top.

## Play

Open `index.html` in a browser, or publish the repo with GitHub Pages from the default branch root.

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
4. Set the source to the default branch and the repository root.
5. Save. GitHub Pages will serve `index.html` automatically.

The expected project-page URL is:

`https://joseprendergast.github.io/The-Last-Drifter/`

## Next Steps

- Continue production in `Unity/TheLastDrifter`.
- Use PowerQuest as the Unity point-and-click framework.
- Treat the HTML game as the story, mood, and scene reference while PowerQuest becomes the production build.
- Build the Alley vertical slice first, then split the Blood Lab / Lavatory and Zoo into proper PowerQuest rooms.
- Replace the imported sample art with bespoke noir character, prop, and room sprites.
