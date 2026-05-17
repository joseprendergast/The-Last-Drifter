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
		Prop("Title").Visible = false;
		Prop("New").Disable();
		Prop("Continue").Disable();
	}

	public IEnumerator OnEnterRoomAfterFade()
	{
		// The custom noir menu is drawn by DrifterActionToolbar. Keep the
		// prototype title props dormant so they don't block the new start flow.
		yield return E.Break;
	}

	public IEnumerator OnInteractPropNew( Prop prop )
	{
		DrifterActionToolbar.PrepareGameplay();
		Globals.ResetDrifterCase();
		Globals.m_drifterProgress = GlobalScript.eDrifterCaseProgress.EnteredAlley;
		G.InventoryBar.Hide();
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
