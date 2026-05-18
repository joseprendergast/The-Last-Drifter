using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;

public class RoomForest : RoomScript<RoomForest>
{
    static readonly Vector2 PlayerStart = new Vector2(-140f, -324f);
    static readonly Vector2 HandStart = new Vector2(115f, -332f);
    int m_rainLooks = 0;

    public void OnEnterRoom()
    {
        bool openingWakeUp = FirstTimeVisited && EnteredFromEditor == false;
        Globals.m_drifterProgress = eDrifterCaseProgress.EnteredAlley;
        DrifterActionToolbar.PrepareGameplay();
        C.Gabardina.SetPosition(PlayerStart, eFace.Right);
        C.Gabardina.Visible = !openingWakeUp;
        C.Barney.Visible = false;
        C.Barney.Clickable = false;
        Prop("Bucket").Position = HandStart;
        Prop("Bucket").Visible = Globals.m_sawSeveredHand == false;
        Prop("Bucket").Clickable = Globals.m_sawSeveredHand == false;
        G.InventoryBar.Hide();
        G.Toolbar.Hide();
    }

    public IEnumerator OnEnterRoomAfterFade()
    {
        if (FirstTimeVisited && EnteredFromEditor == false)
        {
            E.StartCutscene();
            DrifterActionToolbar.BeginWakeUp();
            yield return E.Wait(1.25f);
            C.Gabardina.Visible = true;
            DrifterActionToolbar.EndWakeUp();
            yield return E.Wait(0.18f);
            E.EndCutscene();
            DrifterActionToolbar.ShowToast("Rain over black water. A coat on the floor. One hand where a body should be.");
        }
        else
        {
            C.Gabardina.Visible = true;
        }

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
            prop.Visible = false;
            prop.Clickable = false;
            DrifterActionToolbar.ShowToast("Evidence added: severed hand");
            yield return C.Display("Gabardina opens the paper fold with two fingers. Left hand. Wedding mark. Clean cut.");
            yield return E.WaitSkip();
            yield return C.Display("No blood trail leading away. Somebody placed it here, then waited for him to wake.");
        }
        else
        {
            yield return C.Display("The hand is in the case file now. The empty spot on the floor feels louder.");
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

        if (Globals.m_sawSeveredHand)
        {
            Globals.m_enteredBar = true;
            Globals.m_drifterProgress = eDrifterCaseProgress.EnteredBar;
            E.StartCutscene();
            yield return C.Display("The service door gives on the third shove. Warm light leaks out under it.");
            yield return E.WaitSkip();
            yield return C.Display("Inside: a bar pretending the storm cannot get in.");
            C.Gabardina.Visible = false;
            DrifterActionToolbar.EnterBarInterior();
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
