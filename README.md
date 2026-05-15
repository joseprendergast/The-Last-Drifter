# The Last Drifter

The Last Drifter is a static, browser-playable noir pixel adventure about Captain Gabardina and a city where memories are becoming physical.

`index.html` is the GitHub Pages entry point for the playable cinematic browser slice. It keeps the rain/noir mood, title card, canvas scenes, subtitles, scene selection, contextual hotspots, evidence, save state, settings, and debug tools.

The production game now lives in `Unity/TheLastDrifter`. That project is the Unity + URP + Adventure Creator foundation for the full commercial-grade version.

## Current Browser Slice

- Title screen with Start Game, Continue, Scene Select, Settings, and Reset Progress.
- Three playable scenes: The Alley, The Blood Lab, and The Zoo.
- Contextual canvas hotspots for investigation instead of a visible verb bar.
- Case file and evidence collection.
- Persistent progress with localStorage.
- Scene completion, objective tracking, debug flag inspection, and reset.
- Optional generated ambience/audio after the player starts it.

## Unity Production Direction

- Unity 6.3 LTS or newer Unity 6 LTS.
- Universal Render Pipeline with 2D Renderer.
- Adventure Creator for point-and-click systems.
- Pixel Perfect Camera, TextMeshPro, Input System, and URP post-processing.
- First serious milestone: the Alley vertical slice.

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

- Open `Unity/TheLastDrifter` in Unity Hub.
- Import Adventure Creator through the Unity Asset Store.
- Build the Alley vertical slice first, using the HTML game as the story and mood reference.
- Add more bespoke puzzle logic and dialogue branching inside each Unity scene.
