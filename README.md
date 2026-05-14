# The Last Drifter

The Last Drifter is a static, browser-playable noir pixel adventure about Captain Gabardina and a city where memories are becoming physical.

The next playable build upgrades the original cinematic HTML into a full adventure-game slice with title flow, contextual scene hotspots, evidence collection, persistent progress, settings, optional ambience, and debug tools.

## Current Repository Status

- `index.html` is the GitHub Pages entry point.
- The original cinematic rain, title card, canvas scenes, subtitles, and scene picker are the foundation.
- A new local implementation has been prepared to replace the older prototype with a three-scene playable slice.
- Publishing is complete once the updated `index.html` is committed to the default branch.

## Planned Playable Slice

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

- Commit the updated playable `index.html` to the default branch.
- Split the single-file prototype into scene data, renderer, interaction, save, and audio modules once the slice stabilizes.
- Add more bespoke puzzle logic and dialogue branching inside each scene.
- Add automated browser smoke tests for title flow, scene transitions, evidence persistence, and mobile layout.
