# PowerQuest Migration

The Unity production build now uses PowerQuest 0.20.4 as the adventure framework.

## Current Mapping

- PowerQuest `Title` room acts as The Last Drifter main menu.
- PowerQuest `Forest` room acts as the first rainy alley investigation.
- PowerQuest `Dave` is aliased in code as Captain Gabardina for now.
- The old Unity slice scenes under `Assets/TheLastDrifter/Scenes` remain available as reference scenes.

## First Playable Flow

1. Start from the PowerQuest title room.
2. New Game resets Last Drifter case flags.
3. The player enters the alley room.
4. Investigating the hand sets `m_sawSeveredHand`.
5. Investigating the drain sets `m_foundBloodLavatory`.
6. The service door beat unlocks once both clues are found.

## Next Production Tasks

- Rename or recreate the internal PowerQuest rooms as `MainMenu`, `Alley`, `BloodLab`, and `Zoo`.
- Replace sample props with noir alley sprites: door, severed hand, drain, coat, and rain layers.
- Replace the sample protagonist sprites with Captain Gabardina: long hair, detective coat, heavy collar.
- Add PowerQuest inventory/evidence objects for the hand clue, stained note, memory reel, and zoo ticket.
- Convert the existing HTML narrative beats into PowerQuest room scripts and dialog trees.
