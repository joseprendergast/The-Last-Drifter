using UnityEngine;
using System.Collections;
using PowerScript;
using PowerTools.Quest;
using static GlobalScript;

public class RoomTitle : RoomScript<RoomTitle>
{
	public void OnEnterRoom()
	{
		G.InventoryBar.Hide();
		G.Toolbar.Hide();
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		E.StartCutscene();
		
		Prop("Title").Visible = true;
		yield return Prop("Title").Fade(0,1,1.0f);
		yield return E.Wait(0.5f);

		yield return C.Display("THE LAST DRIFTER");
		yield return E.WaitSkip(0.4f);
		yield return C.Display("Rain over black water. A coat on the floor. One hand where a body should be.");
		
		if (  E.GetSaveSlotData().Count > 0 )
		{
			Prop("Continue").Enable();
			Prop("Continue").FadeBG(0,1,1.0f);
		}
		
		Prop("New").Enable();
		yield return Prop("New").Fade(0,1,1.0f);
		E.EndCutscene();
	}

	public IEnumerator OnInteractPropNew( Prop prop )
	{
		Globals.ResetDrifterCase();
		Globals.m_drifterProgress = GlobalScript.eDrifterCaseProgress.EnteredAlley;
		G.InventoryBar.Show();
		G.Toolbar.Hide();
		
		E.ChangeRoomBG(R.Alley);
		yield return E.ConsumeEvent;
	}

	public IEnumerator OnInteractPropContinue( Prop prop )
	{
		// Restore most recent save game
		E.RestoreLastSave();
		yield return E.ConsumeEvent;
	}

}
