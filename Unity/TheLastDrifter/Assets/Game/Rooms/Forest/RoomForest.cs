using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomForest : RoomScript<RoomForest>
{
    int m_rainLooks = 0;

    public void OnEnterRoom()
    {
        Globals.m_drifterProgress = eDrifterCaseProgress.EnteredAlley;
        G.InventoryBar.Hide();
        G.Toolbar.Hide();
    }

    public IEnumerator OnEnterRoomAfterFade()
    {
        if (FirstTimeVisited && EnteredFromEditor == false)
        {
            yield return C.Display("The alley breathes rain and old electricity.");
            yield return C.Gabardina.WalkTo(Point("EntryWalk"));
            yield return C.Display("Captain Gabardina stops at the edge of the light.");
            yield return E.WaitSkip();
            yield return C.Display("The floor is wet. The coat is empty. The hand is not.");
        }

        C.Gabardina.WalkToBG(Point("EntryWalk"));
        yield return E.Break;
    }

    public IEnumerator OnInteractHotspotForest(Hotspot hotspot)
    {
        yield return C.WalkToClicked();
        yield return C.FaceClicked();

        Globals.m_checkedRain = true;
        yield return C.Display("The walls sweat rainwater and something darker. The city is trying to rinse itself clean.");
        yield return E.Break;
    }

    public IEnumerator OnLookAtHotspotForest(IHotspot hotspot)
    {
        yield return C.FaceClicked();
        yield return C.Display("Brick, rain, and one narrow service door at the far end.");
        yield return E.Break;
    }

    public IEnumerator OnInteractHotspotSky(Hotspot hotspot)
    {
        m_rainLooks++;
        Globals.m_checkedRain = true;
        yield return C.Gabardina.FaceUp();

        if (m_rainLooks == 1)
            yield return C.Display("Rain needles the streetlamp. Every drop looks briefly gold, then black.");
        else
            yield return C.Display("The rain keeps its rhythm. It sounds almost like counting.");

        yield return E.Break;
    }

    public IEnumerator OnInteractPropBucket(Prop prop)
    {
        yield return C.WalkToClicked();
        yield return C.FaceClicked();

        if (Globals.m_sawSeveredHand == false)
        {
            Globals.m_sawSeveredHand = true;
            Globals.m_drifterProgress = eDrifterCaseProgress.SawSeveredHand;
            yield return C.Display("It is a left hand. Wedding mark. Clean cut. No blood trail leading away.");
            yield return E.WaitSkip();
            yield return C.Display("Gabardina wraps it in evidence paper and tries not to think about the missing rest.");
        }
        else
        {
            yield return C.Display("The hand is bagged now, but the shape of it still sits in the rain.");
        }

        yield return E.Break;
    }

    public IEnumerator OnLookAtPropBucket(IProp prop)
    {
        yield return C.FaceClicked();
        yield return C.Display("A severed hand, staged where a clue should be and where a warning usually is.");
        yield return E.Break;
    }

    public IEnumerator OnInteractPropWell(Prop prop)
    {
        yield return C.WalkToClicked();
        yield return C.FaceClicked();

        Globals.m_checkedDrain = true;
        Globals.m_foundBloodLavatory = true;

        if ((int)Globals.m_drifterProgress < (int)eDrifterCaseProgress.FoundBloodDrain)
            Globals.m_drifterProgress = eDrifterCaseProgress.FoundBloodDrain;

        yield return C.Display("The drain coughs up a red thread of water.");
        yield return E.WaitSkip();
        yield return C.Display("Below it, tile gleams under the street. White tile. Lavatory tile. Drowned in blood.");
        yield return E.Break;
    }

    public IEnumerator OnLookAtPropWell(IProp prop)
    {
        yield return C.FaceClicked();
        yield return C.Display("A storm drain, too clean around the edges. Someone opened it recently.");
        yield return E.Break;
    }

    public IEnumerator OnInteractHotspotCave(Hotspot hotspot)
    {
        yield return C.WalkToClicked();
        yield return C.FaceClicked();

        if (Globals.m_sawSeveredHand && Globals.m_foundBloodLavatory)
        {
            Globals.m_drifterProgress = eDrifterCaseProgress.UnlockedLavatory;
            E.StartCutscene();
            yield return C.Display("The service door gives on the third shove.");
            yield return E.WaitSkip();
            yield return C.Display("Below: fluorescent hum, wet tile, and the impossible smell of a hospital at sea.");
            yield return E.WaitSkip();
            yield return C.Display("Next build: Blood Lab / Lavatory room.");
            E.EndCutscene();
        }
        else if (Globals.m_sawSeveredHand == false)
        {
            yield return C.Display("The door can wait. The hand cannot.");
        }
        else
        {
            yield return C.Display("Something under the street is pulling the case downward. Check the drain.");
        }

        yield return E.Break;
    }

    public IEnumerator OnLookAtHotspotCave(IHotspot hotspot)
    {
        yield return C.FaceClicked();
        yield return C.Display("A narrow service door. No handle outside. Scratches around the frame.");
        yield return E.Break;
    }

    public IEnumerator OnUseInvPropWell(Prop prop, Inventory item)
    {
        yield return C.Display("Evidence belongs in the case file. The drain wants something else.");
        yield return E.Break;
    }
}
