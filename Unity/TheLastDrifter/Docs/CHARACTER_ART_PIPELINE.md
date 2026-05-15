# Character Art Pipeline

The current browser slice uses canvas placeholders. The Unity production build should replace those with authored sprites and scene props.

## Recommended Unity Stack

- Adventure framework: Adventure Creator for inventory, hotspots, dialogue, scene logic, saves, and camera flow.
- Character rigging: Unity 2D Animation package for bone-rigged 2D characters, Sprite Library swaps, and reusable animation clips.
- Pixel-art import: Unity Aseprite Importer for `.aseprite` files, preserving animation frames and layer structure.
- Layered painted art: Unity 2D PSD Importer when the character is drawn in Photoshop, Krita, or Clip Studio as a layered `.psb`.
- Presentation: URP 2D Renderer with 2D Lights, Shadow Caster 2D, bloom, color grading, film grain, and vignette.
- Pixel stability: Pixel Perfect Camera for crisp pixel scenes without shimmer.

## Character Targets

### Captain Gabardina

- Silhouette: long dark hair, detective coat, narrow shoulders, heavy collar, old shoes.
- Color read: bone face and hands, dark hair, tan/brown gabardine coat, red undertone in shadows.
- Animation set: idle breathing, walk, kneel, inspect, recoil, talk, hand-to-chest, door-open.
- First scene pose: one standing silhouette in the rain plus one collapsed pose on the wet floor.

### Alley Assets

- Service door: tall metal door with readable handle, key plate, and wet frame.
- Severed hand: large enough to read as a hand at gameplay zoom, with five fingers and blood at the wrist.
- Drain: grate with blood refusing to wash away.
- Props: coat threads, fire escape, puddles, lamp cone, rain streaks.

## Production Rule

Do not use primitive squares for final characters or clue objects. Every readable story object gets either a sprite, a rigged character prefab, or a dedicated prop prefab with a hotspot collider.

