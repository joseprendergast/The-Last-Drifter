using UnityEngine;
using PowerScript;
using PowerTools.Quest;

public class DrifterActionToolbar : MonoBehaviour
{
    public enum ToolMode
    {
        Interact,
        Look,
        Walk
    }

    static readonly Color AlmostBlack = Hex(0x09, 0x0B, 0x0F, 1f);
    static readonly Color DeepNavy = Hex(0x10, 0x18, 0x26, 0.96f);
    static readonly Color Panel = Hex(0x11, 0x18, 0x20, 0.86f);
    static readonly Color Border = Hex(0x42, 0x50, 0x61, 0.72f);
    static readonly Color Amber = Hex(0xD2, 0x8A, 0x35, 1f);
    static readonly Color OffWhite = Hex(0xE2, 0xD8, 0xC4, 1f);
    static readonly Color Secondary = Hex(0x9A, 0xA3, 0xA8, 1f);
    static readonly Color Cyan = Hex(0x8F, 0xC9, 0xD6, 0.84f);
    static readonly Color Blood = Hex(0x5A, 0x0E, 0x12, 0.75f);

    static DrifterActionToolbar s_instance;
    static ToolMode s_mode = ToolMode.Interact;
    static Rect s_toolbarRect;

    GUIStyle m_labelStyle;
    GUIStyle m_menuStyle;
    GUIStyle m_titleStyle;
    bool m_inventoryOpen;
    string m_status = "Inspect";
    string m_hover = string.Empty;
    string m_toast = string.Empty;
    float m_toastUntil = 0f;
    bool m_wakeUpActive;
    float m_wakeUpUntil = 0f;

    readonly ToolButton[] m_buttons =
    {
        new ToolButton("LOOK", "Inspect", ToolMode.Look, ToolButtonKind.Question),
        new ToolButton("CASE", "Case file", ToolMode.Interact, ToolButtonKind.Case),
        new ToolButton("MOVE", "Move", ToolMode.Walk, ToolButtonKind.Walk),
        new ToolButton("ACT", "Interact", ToolMode.Interact, ToolButtonKind.Action)
    };

    readonly string[] m_menuItems = { "NEW GAME", "CONTINUE", "LOAD GAME", "SETTINGS", "CREDITS" };
    readonly string[] m_caseItems = { "SEVERED HAND", "RAINED COAT", "BLOOD DRAIN" };

    public static ToolMode Mode { get { return s_mode; } }

    public static void Ensure()
    {
        if (s_instance != null)
            return;

        GameObject host = new GameObject("Drifter Noir UI");
        DontDestroyOnLoad(host);
        s_instance = host.AddComponent<DrifterActionToolbar>();
    }

    public static bool PointerOverToolbar()
    {
        if (s_toolbarRect.width <= 0f || s_toolbarRect.height <= 0f)
            return false;

        Room room = PowerQuest.Get == null ? null : PowerQuest.Get.GetCurrentRoom();
        if (room == null || room.ScriptName == "Title")
            return false;

        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;
        return s_toolbarRect.Contains(mouse);
    }

    public static void PrepareGameplay()
    {
        Ensure();
        s_mode = ToolMode.Interact;
        s_toolbarRect = Rect.zero;

        if (s_instance != null)
            s_instance.m_inventoryOpen = false;
    }

    public static void ShowToast(string text, float seconds = 3.8f)
    {
        Ensure();
        if (s_instance == null)
            return;

        s_instance.m_toast = text;
        s_instance.m_toastUntil = Time.time + seconds;
    }

    public static void BeginWakeUp()
    {
        Ensure();
        if (s_instance != null)
        {
            s_instance.m_wakeUpActive = true;
            s_instance.m_wakeUpUntil = Time.time + 1.6f;
        }
    }

    public static void EndWakeUp()
    {
        if (s_instance != null)
            s_instance.m_wakeUpActive = false;
    }

    void Awake()
    {
        s_instance = this;
    }

    void OnGUI()
    {
        EnsureStyles();

        Room room = PowerQuest.Get == null ? null : PowerQuest.Get.GetCurrentRoom();
        if (room == null)
            return;

        if (room.ScriptName == "Title")
        {
            s_toolbarRect = new Rect(0, 0, Screen.width, Screen.height);
            DrawTitleMenu();
            return;
        }

        float scale = UiScale();
        DrawSceneLighting(scale);
        DrawSceneRain();
        if (m_wakeUpActive && Time.time < m_wakeUpUntil)
            DrawProneDetective(scale);
        else
            m_wakeUpActive = false;
        DrawMinimalSceneUi();
    }

    void EnsureStyles()
    {
        if (m_labelStyle != null)
            return;

        m_labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            wordWrap = false
        };
        m_labelStyle.normal.textColor = OffWhite;

        m_menuStyle = new GUIStyle(m_labelStyle)
        {
            alignment = TextAnchor.MiddleLeft
        };

        m_titleStyle = new GUIStyle(m_labelStyle)
        {
            alignment = TextAnchor.MiddleLeft
        };
    }

    void DrawTitleMenu()
    {
        float scale = UiScale();
        Event evt = Event.current;
        Vector2 mouse = evt.mousePosition;

        DrawNoirTitleBackground(scale);
        DrawRainField(scale, 70, 0.62f);
        DrawScanlineOverlay(scale, 0.12f);

        float margin = Mathf.Round(Screen.width * 0.15f);
        float titleY = Mathf.Round(Screen.height * 0.22f);
        float panelW = Mathf.Round(252f * scale);
        float panelH = Mathf.Round(276f * scale);
        Rect menuPanel = new Rect(margin - Mathf.Round(20f * scale), titleY - Mathf.Round(20f * scale), panelW, panelH);
        Fill(menuPanel, Hex(0x05, 0x07, 0x0A, 0.52f));
        Stroke(menuPanel, Hex(0x42, 0x50, 0x61, 0.42f), Mathf.Max(1f, Mathf.Round(scale)));
        Fill(new Rect(menuPanel.x, menuPanel.y, Mathf.Round(3f * scale), menuPanel.height), Hex(0xD2, 0x8A, 0x35, 0.78f));

        m_titleStyle.fontSize = Mathf.RoundToInt(22f * scale);
        DrawTextShadow(new Rect(margin, titleY, Screen.width - margin * 2f, Mathf.Round(38f * scale)), "THE LAST DRIFTER", m_titleStyle, OffWhite, scale);

        m_labelStyle.fontSize = Mathf.RoundToInt(7f * scale);
        m_labelStyle.alignment = TextAnchor.MiddleLeft;
        DrawTextShadow(new Rect(margin, titleY + Mathf.Round(40f * scale), Screen.width - margin * 2f, Mathf.Round(18f * scale)), "RAIN / BLACK WATER / ONE HAND LEFT BEHIND", m_labelStyle, Secondary, scale);
        Fill(new Rect(margin, titleY + Mathf.Round(64f * scale), Mathf.Round(130f * scale), Mathf.Max(1f, Mathf.Round(scale))), Amber);
        m_labelStyle.alignment = TextAnchor.MiddleCenter;

        float itemY = titleY + Mathf.Round(90f * scale);
        float itemH = Mathf.Round(24f * scale);
        float itemW = Mathf.Round(176f * scale);
        m_menuStyle.fontSize = Mathf.RoundToInt(10f * scale);

        for (int i = 0; i < m_menuItems.Length; i++)
        {
            Rect r = new Rect(margin, itemY + i * Mathf.Round(31f * scale), itemW, itemH);
            bool over = r.Contains(mouse);
            Color c = over || i == 0 ? Amber : OffWhite;
            if (i > 1)
                c = over ? Secondary : Hex(0x5A, 0x5F, 0x66, 1f);

            if (over)
            {
                Fill(new Rect(r.x - Mathf.Round(12f * scale), r.y + Mathf.Round(2f * scale), r.width + Mathf.Round(18f * scale), r.height - Mathf.Round(4f * scale)), Hex(0xD2, 0x8A, 0x35, 0.13f));
                Fill(new Rect(r.x - Mathf.Round(12f * scale), r.y + Mathf.Round(7f * scale), Mathf.Round(5f * scale), Mathf.Round(10f * scale)), Amber);
            }

            DrawTextShadow(r, m_menuItems[i], m_menuStyle, c, scale);

            if (over && evt.type == EventType.MouseDown && evt.button == 0)
            {
                ActivateMenuItem(i);
                evt.Use();
            }
        }

        m_labelStyle.fontSize = Mathf.RoundToInt(7f * scale);
        m_labelStyle.alignment = TextAnchor.MiddleLeft;
        DrawTextShadow(new Rect(margin, Screen.height - Mathf.Round(44f * scale), Screen.width - margin * 2f, Mathf.Round(18f * scale)), "Click or press A to enter the alley. Right click inspects. TAB reveals hotspots.", m_labelStyle, Secondary, scale);
        m_labelStyle.alignment = TextAnchor.MiddleCenter;
    }

    void ActivateMenuItem(int index)
    {
        if (PowerQuest.Get == null)
            return;

        if (index == 0)
        {
            PrepareGameplay();
            GlobalScript.Script.ResetDrifterCase();
            G.InventoryBar.Hide();
            G.Toolbar.Hide();
            PowerQuest.Get.ChangeRoomBG(PowerQuest.Get.GetRoom("Forest"));
            return;
        }

        if (index == 1)
        {
            PowerQuest.Get.RestoreLastSave();
            return;
        }

        if (index == 3)
        {
            G.Options.Show();
            return;
        }

        m_status = index == 2 ? "No save selected." : "Credits are still in the case file.";
    }

    void DrawNoirTitleBackground(float scale)
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), AlmostBlack);

        for (int y = 0; y < Screen.height; y += Mathf.Max(1, Mathf.RoundToInt(3f * scale)))
        {
            float t = y / Mathf.Max(1f, Screen.height);
            Fill(new Rect(0, y, Screen.width, Mathf.Max(1f, 3f * scale)), Color.Lerp(DeepNavy, AlmostBlack, t * 0.8f));
        }

        float horizon = Mathf.Round(Screen.height * 0.55f);
        Fill(new Rect(0, horizon, Screen.width, Screen.height - horizon), Hex(0x05, 0x07, 0x0A, 0.72f));

        DrawBuilding(new Rect(Screen.width * 0.55f, Screen.height * 0.18f, Screen.width * 0.22f, Screen.height * 0.46f), scale);
        DrawBuilding(new Rect(Screen.width * 0.77f, Screen.height * 0.12f, Screen.width * 0.16f, Screen.height * 0.58f), scale);
        DrawBuilding(new Rect(Screen.width * 0.38f, Screen.height * 0.24f, Screen.width * 0.14f, Screen.height * 0.40f), scale);

        Color light = Hex(0xD2, 0x8A, 0x35, 0.13f);
        DrawQuad(new Vector2(Screen.width * 0.48f, Screen.height * 0.38f), new Vector2(Screen.width * 0.70f, Screen.height * 0.88f), new Vector2(Screen.width * 0.35f, Screen.height * 0.88f), light, scale);
        Fill(new Rect(Screen.width * 0.44f, Screen.height * 0.36f, Screen.width * 0.13f, Mathf.Round(5f * scale)), Hex(0xD2, 0x8A, 0x35, 0.55f));
        Fill(new Rect(Screen.width * 0.49f, Screen.height * 0.37f, Mathf.Round(9f * scale), Mathf.Round(15f * scale)), Hex(0xE2, 0xD8, 0xC4, 0.62f));

        for (int i = 0; i < 9; i++)
        {
            float x = Mathf.Repeat(Time.time * 23f + i * 97f, Screen.width);
            Fill(new Rect(x, Screen.height * 0.75f + Mathf.Sin(Time.time + i) * 8f, Mathf.Round(60f * scale), Mathf.Max(1f, Mathf.Round(2f * scale))), Hex(0x8F, 0xC9, 0xD6, 0.12f));
        }

        DrawVignette(scale, 0.68f);
    }

    void DrawBuilding(Rect r, float scale)
    {
        Fill(r, Hex(0x0A, 0x0F, 0x16, 0.94f));
        Fill(new Rect(r.x, r.y, r.width, Mathf.Round(4f * scale)), Hex(0x2D, 0x3E, 0x52, 0.4f));
        for (float y = r.y + Mathf.Round(24f * scale); y < r.yMax - Mathf.Round(8f * scale); y += Mathf.Round(32f * scale))
        {
            Fill(new Rect(r.x + r.width * 0.14f, y, r.width * 0.18f, Mathf.Round(3f * scale)), Hex(0x3F, 0xA6, 0xA1, 0.24f));
            Fill(new Rect(r.x + r.width * 0.54f, y + Mathf.Round(9f * scale), r.width * 0.22f, Mathf.Round(3f * scale)), Hex(0xD2, 0x8A, 0x35, 0.17f));
        }
    }

    void DrawMinimalSceneUi()
    {
        float scale = UiScale();
        Event evt = Event.current;
        Vector2 mouse = evt.mousePosition;

        m_hover = string.Empty;
        string text = GetDisplayText();
        if (!string.IsNullOrEmpty(text) && (!PowerQuest.Get.GetBlocked() || !string.IsNullOrEmpty(m_hover)))
            DrawSceneLabel(text, scale);

        DrawCompactDock(scale, mouse, evt);
        if (m_inventoryOpen)
            DrawCaseStrip(scale, mouse, evt);

        DrawScanlineOverlay(scale, 0.06f);
    }

    void DrawCompactDock(float scale, Vector2 mouse, Event evt)
    {
        float slot = Mathf.Round(46f * scale);
        float gap = Mathf.Round(7f * scale);
        float bottomPad = Mathf.Round(14f * scale);
        float dockWidth = m_buttons.Length * slot + (m_buttons.Length - 1) * gap;
        float startX = Mathf.Round((Screen.width - dockWidth) * 0.5f);
        float startY = Mathf.Round(Screen.height - bottomPad - slot);

        s_toolbarRect = new Rect(startX - Mathf.Round(8f * scale), startY - Mathf.Round(8f * scale), dockWidth + Mathf.Round(16f * scale), slot + Mathf.Round(16f * scale));

        Fill(new Rect(s_toolbarRect.x, s_toolbarRect.y, s_toolbarRect.width, s_toolbarRect.height), Hex(0x05, 0x07, 0x0A, 0.34f));

        for (int i = 0; i < m_buttons.Length; i++)
        {
            Rect slotRect = new Rect(startX + i * (slot + gap), startY, slot, slot);
            DrawButton(slotRect, m_buttons[i], scale, mouse, evt);
        }
    }

    void DrawCaseStrip(float scale, Vector2 mouse, Event evt)
    {
        float w = Mathf.Round(290f * scale);
        float h = Mathf.Round(44f * scale);
        Rect strip = new Rect(Mathf.Round((Screen.width - w) * 0.5f), Screen.height - Mathf.Round(108f * scale), w, h);
        Fill(strip, Panel);
        Stroke(strip, Border, Mathf.Max(2f, Mathf.Round(2f * scale)));

        m_labelStyle.fontSize = Mathf.RoundToInt(6f * scale);
        m_labelStyle.alignment = TextAnchor.MiddleLeft;
        DrawTextShadow(new Rect(strip.x + Mathf.Round(12f * scale), strip.y + Mathf.Round(4f * scale), strip.width, Mathf.Round(12f * scale)), "CASE FILE", m_labelStyle, Amber, scale);

        for (int i = 0; i < m_caseItems.Length; i++)
        {
            Rect item = new Rect(strip.x + Mathf.Round(12f * scale) + i * Mathf.Round(88f * scale), strip.y + Mathf.Round(20f * scale), Mathf.Round(78f * scale), Mathf.Round(16f * scale));
            bool over = item.Contains(mouse);
            if (over)
                Fill(item, Hex(0xD2, 0x8A, 0x35, 0.16f));
            DrawTextShadow(item, m_caseItems[i], m_labelStyle, over ? OffWhite : Secondary, scale);
        }

        m_labelStyle.alignment = TextAnchor.MiddleCenter;
        s_toolbarRect = Rect.MinMaxRect(Mathf.Min(s_toolbarRect.xMin, strip.xMin), Mathf.Min(s_toolbarRect.yMin, strip.yMin), Mathf.Max(s_toolbarRect.xMax, strip.xMax), Mathf.Max(s_toolbarRect.yMax, strip.yMax));
    }

    void DrawButton(Rect rect, ToolButton button, float scale, Vector2 mouse, Event evt)
    {
        bool over = rect.Contains(mouse);
        bool active = button.Mode == s_mode && button.Kind != ToolButtonKind.Case;
        if (button.Kind == ToolButtonKind.Action)
            active = s_mode == ToolMode.Interact;
        if (button.Kind == ToolButtonKind.Case)
            active = m_inventoryOpen;

        DrawSlot(rect, active, over, scale);

        Color main = active ? Amber : (over ? OffWhite : Secondary);
        DrawIcon(rect, button.Kind, main, scale, active);

        if (over)
        {
            m_hover = button.Label;
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                Activate(button);
                evt.Use();
            }
        }
    }

    void DrawSlot(Rect rect, bool active, bool over, float scale)
    {
        float b = Mathf.Max(2f, Mathf.Round(2f * scale));
        Color outer = active ? OffWhite : Border;
        Color inner = active ? Hex(0x11, 0x18, 0x20, 0.94f) : Hex(0x05, 0x07, 0x0A, 0.58f);

        Fill(new Rect(rect.x + b, rect.y + b, rect.width, rect.height), Hex(0x00, 0x00, 0x00, 0.42f));
        Fill(rect, outer);
        Fill(new Rect(rect.x + b, rect.y + b, rect.width - b * 2f, rect.height - b * 2f), over && !active ? Panel : inner);
        if (active)
            Stroke(new Rect(rect.x + b * 2f, rect.y + b * 2f, rect.width - b * 4f, rect.height - b * 4f), Amber, b);
    }

    void Activate(ToolButton button)
    {
        if (button.Kind == ToolButtonKind.Case)
        {
            m_inventoryOpen = !m_inventoryOpen;
            G.InventoryBar.Hide();
            m_status = m_inventoryOpen ? "Case file" : "Inspect";
            return;
        }

        m_inventoryOpen = false;
        s_mode = button.Mode;
        m_status = button.Label;
    }

    string GetDisplayText()
    {
        if (!string.IsNullOrEmpty(m_hover))
            return m_hover;

        string sceneHover = GetSceneHoverText();
        if (!string.IsNullOrEmpty(sceneHover))
            return sceneHover;

        return Time.time < m_toastUntil ? m_toast : string.Empty;
    }

    string GetSceneHoverText()
    {
        if (PowerQuest.Get == null || PowerQuest.Get.GetBlocked())
            return string.Empty;

        string description = PowerQuest.Get.GetMouseOverDescription();
        return string.IsNullOrEmpty(description) ? string.Empty : description;
    }

    void DrawSceneLabel(string text, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return;

        m_labelStyle.fontSize = Mathf.RoundToInt(9f * scale);
        Rect rect = new Rect(Mathf.Round(Screen.width * 0.15f), Mathf.Round(18f * scale), Mathf.Round(Screen.width * 0.70f), Mathf.Round(20f * scale));
        Fill(new Rect(rect.x - Mathf.Round(8f * scale), rect.y - Mathf.Round(2f * scale), rect.width + Mathf.Round(16f * scale), rect.height + Mathf.Round(4f * scale)), Hex(0x05, 0x07, 0x0A, 0.36f));
        DrawTextShadow(rect, text, m_labelStyle, OffWhite, scale);
    }

    void DrawIcon(Rect r, ToolButtonKind kind, Color c, float scale, bool active)
    {
        float u = Mathf.Max(1f, Mathf.Round(2f * scale));
        Rect inner = new Rect(r.x + r.width * 0.25f, r.y + r.height * 0.22f, r.width * 0.50f, r.height * 0.56f);
        Color ink = Hex(0x09, 0x0B, 0x0F, 0.96f);
        Color fill = active ? OffWhite : c;

        switch (kind)
        {
            case ToolButtonKind.Question:
                Fill(new Rect(inner.x + u * 2, inner.y, inner.width - u * 4, u * 3), fill);
                Fill(new Rect(inner.x + inner.width - u * 4, inner.y + u * 2, u * 4, u * 4), fill);
                Fill(new Rect(inner.x + inner.width * 0.45f, inner.y + u * 6, u * 3, u * 6), fill);
                Fill(new Rect(inner.x + inner.width * 0.45f, inner.y + inner.height - u * 3, u * 3, u * 3), fill);
                break;
            case ToolButtonKind.Case:
                Stroke(new Rect(inner.x + u * 2, inner.y + u * 5, inner.width - u * 4, inner.height - u * 8), fill, u * 2);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 2, inner.width - u * 10, u * 3), fill);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 10, inner.width - u * 10, u * 2), fill);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 15, inner.width - u * 13, u * 2), fill);
                Fill(new Rect(inner.x + inner.width - u * 7, inner.y + u * 15, u * 3, u * 2), active ? Amber : Cyan);
                break;
            case ToolButtonKind.Walk:
                Fill(new Rect(inner.x + u * 8, inner.y + u * 2, u * 5, u * 9), fill);
                Fill(new Rect(inner.x + u * 6, inner.y + u * 10, u * 6, u * 8), fill);
                Fill(new Rect(inner.x + u * 3, inner.y + u * 17, u * 13, u * 3), fill);
                Fill(new Rect(inner.x + u, inner.y + u * 20, u * 17, u * 3), fill);
                Fill(new Rect(inner.x + u * 13, inner.y + u * 14, u * 4, u * 3), fill);
                break;
            case ToolButtonKind.Action:
                Fill(new Rect(inner.x + u * 6, inner.y + u * 4, u * 4, u * 14), fill);
                Fill(new Rect(inner.x + u * 10, inner.y + u * 7, u * 3, u * 10), fill);
                Fill(new Rect(inner.x + u * 13, inner.y + u * 9, u * 3, u * 8), fill);
                Fill(new Rect(inner.x + u * 3, inner.y + u * 11, u * 4, u * 7), fill);
                Fill(new Rect(inner.x + u * 4, inner.y + u * 17, u * 12, u * 5), fill);
                Fill(new Rect(inner.x + u * 7, inner.y + u * 21, u * 7, u * 3), fill);
                break;
        }
    }

    void DrawSceneRain()
    {
        DrawRainField(UiScale(), 18, 0.20f);
    }

    void DrawRainField(float scale, int drops, float opacity)
    {
        Color rain = Hex(0x8F, 0xC9, 0xD6, opacity);
        float speed = Time.time * 165f;
        for (int i = 0; i < drops; i++)
        {
            float seed = i * 73.217f;
            float x = Mathf.Repeat(seed * 17f + speed * 0.22f, Screen.width + 80f) - 40f;
            float y = Mathf.Repeat(seed * 29f + speed * (0.55f + (i % 4) * 0.06f), Screen.height + 90f) - 50f;
            float len = Mathf.Round((10f + (i % 5) * 4f) * scale);
            Fill(new Rect(x, y, Mathf.Max(1f, scale), len), rain);
        }
    }

    void DrawSceneLighting(float scale)
    {
        DrawQuad(
            new Vector2(Screen.width * 0.14f, Screen.height * 0.46f),
            new Vector2(Screen.width * 0.62f, Screen.height * 0.78f),
            new Vector2(Screen.width * 0.03f, Screen.height * 0.86f),
            Hex(0xD2, 0x8A, 0x35, 0.15f),
            scale);

        Fill(new Rect(0, Screen.height * 0.64f, Screen.width * 0.78f, Mathf.Round(44f * scale)), Hex(0xD2, 0x8A, 0x35, 0.06f));
        Fill(new Rect(Screen.width * 0.54f, Screen.height * 0.52f, Screen.width * 0.38f, Mathf.Round(5f * scale)), Hex(0x8F, 0xC9, 0xD6, 0.07f));
        Fill(new Rect(Screen.width * 0.75f, Screen.height * 0.42f, Mathf.Round(4f * scale), Screen.height * 0.30f), Hex(0xD2, 0x8A, 0x35, 0.22f));
        DrawVignette(scale, 0.42f);
    }

    void DrawProneDetective(float scale)
    {
        float u = Mathf.Max(2f, Mathf.Round(3f * scale));
        float x = Mathf.Round(Screen.width * 0.38f);
        float y = Mathf.Round(Screen.height * 0.76f);
        Color coat = Hex(0x0B, 0x0F, 0x14, 0.94f);
        Color shirt = Hex(0xE2, 0xD8, 0xC4, 0.72f);
        Color skin = Hex(0xC4, 0x5A, 0x28, 0.72f);

        Fill(new Rect(x - u * 10f, y + u * 7f, u * 34f, u * 3f), Hex(0x00, 0x00, 0x00, 0.34f));
        Fill(new Rect(x, y, u * 22f, u * 7f), coat);
        Fill(new Rect(x + u * 4f, y + u * 2f, u * 9f, u * 2f), shirt);
        Fill(new Rect(x - u * 5f, y + u, u * 6f, u * 6f), coat);
        Fill(new Rect(x + u * 22f, y + u * 2f, u * 5f, u * 4f), skin);
        Fill(new Rect(x + u * 6f, y + u * 7f, u * 15f, u * 3f), coat);
    }

    void DrawScanlineOverlay(float scale, float alpha)
    {
        float step = Mathf.Max(2f, Mathf.Round(3f * scale));
        for (float y = 0; y < Screen.height; y += step)
            Fill(new Rect(0, y, Screen.width, Mathf.Max(1f, scale)), Hex(0x00, 0x00, 0x00, alpha));
    }

    void DrawVignette(float scale, float alpha)
    {
        float band = Mathf.Round(44f * scale);
        Fill(new Rect(0, 0, Screen.width, band), Hex(0x00, 0x00, 0x00, alpha));
        Fill(new Rect(0, Screen.height - band, Screen.width, band), Hex(0x00, 0x00, 0x00, alpha));
        Fill(new Rect(0, 0, Mathf.Round(38f * scale), Screen.height), Hex(0x00, 0x00, 0x00, alpha * 0.72f));
        Fill(new Rect(Screen.width - Mathf.Round(38f * scale), 0, Mathf.Round(38f * scale), Screen.height), Hex(0x00, 0x00, 0x00, alpha * 0.72f));
    }

    void DrawTextShadow(Rect rect, string text, GUIStyle style, Color color, float scale)
    {
        Color oldText = style.normal.textColor;
        style.normal.textColor = Hex(0x00, 0x00, 0x00, 0.86f);
        GUI.Label(new Rect(rect.x + Mathf.Round(2f * scale), rect.y + Mathf.Round(2f * scale), rect.width, rect.height), text, style);
        style.normal.textColor = color;
        GUI.Label(rect, text, style);
        style.normal.textColor = oldText;
    }

    void DrawQuad(Vector2 a, Vector2 b, Vector2 c, Color color, float scale)
    {
        int steps = Mathf.RoundToInt(28f * scale);
        for (int i = 0; i < steps; i++)
        {
            float t0 = i / (float)steps;
            float t1 = (i + 1) / (float)steps;
            Vector2 l0 = Vector2.Lerp(a, b, t0);
            Vector2 r0 = Vector2.Lerp(a, c, t0);
            Vector2 l1 = Vector2.Lerp(a, b, t1);
            Vector2 r1 = Vector2.Lerp(a, c, t1);
            Fill(new Rect(Mathf.Min(l0.x, r0.x), Mathf.Min(l0.y, l1.y), Mathf.Abs(r0.x - l0.x), Mathf.Max(1f, Mathf.Abs(l1.y - l0.y))), color);
        }
    }

    void Stroke(Rect rect, Color color, float width)
    {
        Fill(new Rect(rect.x, rect.y, rect.width, width), color);
        Fill(new Rect(rect.x, rect.yMax - width, rect.width, width), color);
        Fill(new Rect(rect.x, rect.y, width, rect.height), color);
        Fill(new Rect(rect.xMax - width, rect.y, width, rect.height), color);
    }

    void Fill(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = old;
    }

    float UiScale()
    {
        return Mathf.Clamp(Screen.height / 600f, 1f, 2f);
    }

    static Color Hex(int r, int g, int b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    struct ToolButton
    {
        public readonly string Name;
        public readonly string Label;
        public readonly ToolMode Mode;
        public readonly ToolButtonKind Kind;

        public ToolButton(string name, string label, ToolMode mode, ToolButtonKind kind)
        {
            Name = name;
            Label = label;
            Mode = mode;
            Kind = kind;
        }
    }

    enum ToolButtonKind
    {
        Question,
        Case,
        Walk,
        Action
    }
}
