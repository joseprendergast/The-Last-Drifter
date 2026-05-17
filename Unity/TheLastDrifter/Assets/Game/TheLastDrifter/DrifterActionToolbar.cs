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

    static DrifterActionToolbar s_instance;
    static ToolMode s_mode = ToolMode.Interact;
    static Rect s_toolbarRect;

    GUIStyle m_labelStyle;
    string m_status = "Use / Inspect";
    string m_hover = string.Empty;

    readonly ToolButton[] m_buttons =
    {
        new ToolButton("Question", "Look closer", ToolMode.Look, ToolButtonKind.Question),
        new ToolButton("Evidence", "Inspect evidence", ToolMode.Interact, ToolButtonKind.HandClue),
        new ToolButton("Case", "Evidence drawer", ToolMode.Interact, ToolButtonKind.Keycard),
        new ToolButton("Use", "Use / Inspect", ToolMode.Interact, ToolButtonKind.Detective),
        new ToolButton("Walk", "Walk", ToolMode.Walk, ToolButtonKind.Boot)
    };

    public static ToolMode Mode { get { return s_mode; } }

    public static void Ensure()
    {
        if (s_instance != null)
            return;

        GameObject host = new GameObject("Drifter Action Toolbar");
        DontDestroyOnLoad(host);
        s_instance = host.AddComponent<DrifterActionToolbar>();
    }

    public static bool PointerOverToolbar()
    {
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;
        return s_toolbarRect.Contains(mouse);
    }

    void Awake()
    {
        s_instance = this;
    }

    void OnGUI()
    {
        if (ShouldHide())
            return;

        EnsureStyles();

        float scale = Mathf.Clamp(Screen.height / 600f, 1f, 2.25f);
        float slot = Mathf.Round(92f * scale);
        float gap = Mathf.Round(10f * scale);
        float bottomPad = Mathf.Round(24f * scale);
        float dockWidth = m_buttons.Length * slot + (m_buttons.Length - 1) * gap;
        float startX = Mathf.Round((Screen.width - dockWidth) * 0.5f);
        float startY = Mathf.Round(Screen.height - bottomPad - slot);

        s_toolbarRect = new Rect(startX - Mathf.Round(10f * scale), startY - Mathf.Round(8f * scale), dockWidth + Mathf.Round(20f * scale), slot + Mathf.Round(18f * scale));

        Event evt = Event.current;
        Vector2 mouse = evt.mousePosition;
        m_hover = string.Empty;

        DrawTopFrame(scale);
        DrawSceneLabel(GetDisplayText(), scale);

        for (int i = 0; i < m_buttons.Length; i++)
        {
            Rect slotRect = new Rect(startX + i * (slot + gap), startY, slot, slot);
            DrawButton(slotRect, m_buttons[i], scale, mouse, evt);
        }
    }

    bool ShouldHide()
    {
        if (PowerQuest.Get == null)
            return true;

        Room room = PowerQuest.Get.GetCurrentRoom();
        return room == null || room.ScriptName == "Title";
    }

    void EnsureStyles()
    {
        if (m_labelStyle != null)
            return;

        m_labelStyle = new GUIStyle(GUI.skin.label);
        m_labelStyle.alignment = TextAnchor.MiddleCenter;
        m_labelStyle.normal.textColor = new Color(0.82f, 0.78f, 0.66f);
        m_labelStyle.fontStyle = FontStyle.Bold;
        m_labelStyle.wordWrap = false;
    }

    string GetDisplayText()
    {
        if (!string.IsNullOrEmpty(m_hover))
            return m_hover;

        string sceneHover = GetSceneHoverText();
        return !string.IsNullOrEmpty(sceneHover) ? sceneHover : m_status;
    }

    void DrawTopFrame(float scale)
    {
        float h = Mathf.Round(18f * scale);
        float edge = Mathf.Max(3f, Mathf.Round(4f * scale));
        float notchW = Mathf.Round(62f * scale);
        float notchH = Mathf.Round(22f * scale);
        Color shadow = new Color(0.01f, 0.018f, 0.026f, 0.88f);
        Color edgeColor = new Color(0.20f, 0.34f, 0.33f, 0.96f);
        Color dark = new Color(0.025f, 0.05f, 0.065f, 0.86f);

        Fill(new Rect(0, 0, Screen.width, h), shadow);
        Fill(new Rect(0, h - edge, Screen.width, edge), edgeColor);

        DrawTopNotch(Mathf.Round(Screen.width * 0.12f), h, notchW, notchH, edge, dark, edgeColor);
        DrawTopNotch(Mathf.Round(Screen.width * 0.50f - notchW * 0.5f), h, notchW, notchH, edge, dark, edgeColor);
        DrawTopNotch(Mathf.Round(Screen.width * 0.88f - notchW), h, notchW, notchH, edge, dark, edgeColor);
    }

    void DrawTopNotch(float x, float y, float w, float h, float edge, Color dark, Color edgeColor)
    {
        Fill(new Rect(x, y - edge, w, h), edgeColor);
        Fill(new Rect(x + edge, y - edge, w - edge * 2f, h - edge), dark);
        Fill(new Rect(x + w * 0.42f, y + h * 0.32f, w * 0.16f, h * 0.16f), new Color(0.30f, 0.43f, 0.41f, 1f));
    }

    void DrawSceneLabel(string text, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return;

        m_labelStyle.fontSize = Mathf.RoundToInt(26f * scale);
        float y = Mathf.Round(22f * scale);
        float h = Mathf.Round(38f * scale);
        Rect shadow = new Rect(0, y + Mathf.Round(3f * scale), Screen.width, h);
        Rect rect = new Rect(0, y, Screen.width, h);
        Color old = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.82f);
        GUI.Label(shadow, text, m_labelStyle);
        GUI.color = Color.white;
        GUI.Label(rect, text, m_labelStyle);
        GUI.color = old;
    }

    void DrawButton(Rect rect, ToolButton button, float scale, Vector2 mouse, Event evt)
    {
        bool over = rect.Contains(mouse);
        bool active = button.Mode == s_mode && button.Kind != ToolButtonKind.Keycard;
        if (button.Kind == ToolButtonKind.Detective)
            active = s_mode == ToolMode.Interact;

        DrawSlot(rect, active, over, scale);

        Color main = over || active ? new Color(0.72f, 0.88f, 0.86f, 1f) : new Color(0.27f, 0.43f, 0.43f, 1f);
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
        float b = Mathf.Max(4f, Mathf.Round(5f * scale));
        Color outer = active ? new Color(0.86f, 0.93f, 0.90f, 1f) : new Color(0.12f, 0.24f, 0.25f, 0.92f);
        Color mid = active ? new Color(0.25f, 0.72f, 0.73f, 1f) : new Color(0.24f, 0.31f, 0.32f, 0.9f);
        Color inner = active ? new Color(0.08f, 0.19f, 0.19f, 0.96f) : new Color(0.04f, 0.055f, 0.07f, 0.82f);

        Fill(new Rect(rect.x + b, rect.y + b, rect.width, rect.height), new Color(0f, 0f, 0f, 0.55f));
        Fill(rect, outer);
        Fill(new Rect(rect.x + b, rect.y + b, rect.width - b * 2f, rect.height - b * 2f), mid);
        Fill(new Rect(rect.x + b * 2f, rect.y + b * 2f, rect.width - b * 4f, rect.height - b * 4f), inner);

        if (!active && !over)
            Fill(new Rect(rect.x + b * 2f, rect.y + b * 2f, rect.width - b * 4f, rect.height - b * 4f), new Color(0f, 0f, 0f, 0.34f));

        Fill(new Rect(rect.x + b * 2f, rect.y + b * 2f, rect.width - b * 4f, Mathf.Max(2f, b * 0.5f)), new Color(0.42f, 0.56f, 0.54f, active ? 0.85f : 0.38f));
        Fill(new Rect(rect.x + b * 2f, rect.yMax - b * 2.5f, rect.width - b * 4f, b), new Color(0f, 0f, 0f, 0.58f));
    }

    void Activate(ToolButton button)
    {
        if (button.Kind == ToolButtonKind.Keycard)
        {
            G.InventoryBar.Show();
            m_status = "Evidence drawer";
            return;
        }

        s_mode = button.Mode;
        m_status = button.Label;
    }

    string GetSceneHoverText()
    {
        if (PowerQuest.Get == null || PowerQuest.Get.GetBlocked())
            return string.Empty;

        string description = PowerQuest.Get.GetMouseOverDescription();
        return string.IsNullOrEmpty(description) ? string.Empty : description;
    }

    void DrawIcon(Rect r, ToolButtonKind kind, Color c, float scale, bool active)
    {
        float u = Mathf.Max(2f, Mathf.Round(4f * scale));
        Rect inner = new Rect(r.x + r.width * 0.17f, r.y + r.height * 0.14f, r.width * 0.66f, r.height * 0.70f);
        Color ink = new Color(0.01f, 0.018f, 0.025f, 1f);
        Color highlight = active ? Color.white : new Color(0.55f, 0.70f, 0.68f, 1f);

        switch (kind)
        {
            case ToolButtonKind.Question:
                Fill(new Rect(inner.x + u * 2, inner.y + u * 1, inner.width - u * 4, u * 5), c);
                Fill(new Rect(inner.x + inner.width - u * 5, inner.y + u * 4, u * 5, u * 5), c);
                Fill(new Rect(inner.x + inner.width * 0.46f, inner.y + u * 8, u * 5, u * 8), c);
                Fill(new Rect(inner.x + inner.width * 0.46f, inner.y + inner.height - u * 5, u * 5, u * 5), c);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 5, inner.width - u * 10, u * 4), ink);
                break;
            case ToolButtonKind.HandClue:
                Fill(new Rect(inner.x + u * 2, inner.y + u * 10, inner.width - u * 4, u * 7), c);
                Fill(new Rect(inner.x + u * 4, inner.y + u * 6, inner.width - u * 8, u * 6), new Color(0.45f, 0.58f, 0.58f, 1f));
                Fill(new Rect(inner.x + u * 6, inner.y + u * 3, u * 13, u * 5), highlight);
                Fill(new Rect(inner.x + u * 14, inner.y + u * 1, u * 7, u * 5), highlight);
                Fill(new Rect(inner.x + u * 20, inner.y + u * 5, u * 5, u * 6), c);
                Fill(new Rect(inner.x + u * 1, inner.y + u * 14, u * 8, u * 3), highlight);
                break;
            case ToolButtonKind.Keycard:
                Fill(new Rect(inner.x + u * 3, inner.y + u * 4, inner.width - u * 6, inner.height - u * 8), new Color(0.13f, 0.22f, 0.24f, 1f));
                Fill(new Rect(inner.x + u * 5, inner.y + u * 6, inner.width - u * 10, u * 4), c);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 13, inner.width - u * 12, u * 3), new Color(0.35f, 0.48f, 0.48f, 1f));
                Fill(new Rect(inner.x + inner.width - u * 11, inner.y + inner.height - u * 13, u * 6, u * 6), highlight);
                break;
            case ToolButtonKind.Detective:
                Fill(new Rect(inner.x + u * 8, inner.y + u * 1, u * 11, u * 7), highlight);
                Fill(new Rect(inner.x + u * 7, inner.y + u * 7, u * 13, u * 8), ink);
                Fill(new Rect(inner.x + u * 3, inner.y + u * 14, u * 18, u * 16), c);
                Fill(new Rect(inner.x + u * 3, inner.y + u * 14, u * 7, u * 16), new Color(0.27f, 0.45f, 0.45f, 1f));
                Fill(new Rect(inner.x + u * 17, inner.y + u * 13, u * 9, u * 5), highlight);
                Fill(new Rect(inner.x + u * 20, inner.y + u * 17, u * 5, u * 12), ink);
                Fill(new Rect(inner.x + u * 8, inner.y + u * 22, u * 14, u * 5), highlight);
                break;
            case ToolButtonKind.Boot:
                Fill(new Rect(inner.x + u * 9, inner.y + u * 2, u * 8, u * 22), c);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 21, u * 20, u * 7), c);
                Fill(new Rect(inner.x + u * 2, inner.y + u * 25, u * 14, u * 5), highlight);
                Fill(new Rect(inner.x + u * 8, inner.y + u * 24, u * 15, u * 3), ink);
                break;
        }
    }

    void Fill(Rect rect, Color color)
    {
        Color old = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = old;
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
        HandClue,
        Keycard,
        Detective,
        Boot
    }
}
