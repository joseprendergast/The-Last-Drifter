using UnityEngine;
using System.Collections;
using PowerTools.Quest;
using PowerScript;
using static GlobalScript;
using PowerTools.QuestGui;

public class GuiHoverText : GuiScript<GuiHoverText>
{

	// Edit these to change offsets of cursor

	// Offset of text from mouse cursor
	Vector2 m_offsetFromCursor = new Vector2(35,-35);
	// How close it can get to edge of screen
	float m_offsetFromEdge = 20;


	void Update()
	{
		// Move text with cursor- Comment this out if you want it static
		UpdateTextOnCursorPos();

		// Update text
		Label("Text").Text= E.GetMouseOverDescription();		
		Label("Text").Visible = !E.GetBlocked();
		
	}

	void UpdateTextOnCursorPos()
	{ 		
		// Set position, and prevent going off screen
		RectCentered textRect = (Label("Text") as GuiControl).GetRect();
		float maxX = (E.GetCameraGui().GetWidth()*0.5f)  - m_offsetFromEdge;
		float maxY = (E.GetCameraGui().GetHeight()*0.5f) - m_offsetFromEdge;
		
		Gui.Position = E.GetMousePositionGui() + m_offsetFromCursor;
		Gui.Position = new Vector2(
			Mathf.Min(Gui.Position.x, maxX-textRect.Width),
			Mathf.Max(Gui.Position.y, -maxY+textRect.Height));
		
	}
}
