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
    GUIStyle m_smallStyle;
    string m_status = "Use";
    string m_hover = string.Empty;

    readonly ToolButton[] m_buttons =
    {
        new ToolButton("Use", "Use / Inspect", ToolMode.Interact, ToolButtonKind.Hand),
        new ToolButton("Look", "Look closer", ToolMode.Look, ToolButtonKind.Eye),
        new ToolButton("Walk", "Walk", ToolMode.Walk, ToolButtonKind.Boot),
        new ToolButton("Case", "Evidence drawer", ToolMode.Interact, ToolButtonKind.File),
        new ToolButton("Save", "Save game", ToolMode.Interact, ToolButtonKind.Card),
        new ToolButton("Gear", "Options", ToolMode.Interact, ToolButtonKind.Gear)
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

        float scale = Mathf.Clamp(Screen.height / 600f, 1f, 2f);
        float barHeight = Mathf.Round(74f * scale);
        float border = Mathf.Max(4f, Mathf.Round(5f * scale));
        float cell = Mathf.Round(48f * scale);
        float gap = Mathf.Round(8f * scale);
        float startX = Mathf.Round(36f * scale);
        float top = 0f;
        float labelY = Mathf.Round(78f * scale);

        s_toolbarRect = new Rect(0, 0, Screen.width, barHeight + Mathf.Round(12f * scale));

        DrawBarFrame(barHeight, border, scale);

        Event evt = Event.current;
        Vector2 mouse = evt.mousePosition;
        m_hover = string.Empty;

        for (int i = 0; i < m_buttons.Length; i++)
        {
            Rect cellRect = new Rect(startX + i * (cell + gap), top + Mathf.Round(9f * scale), cell, cell);
            DrawButton(cellRect, m_buttons[i], scale, mouse, evt);
        }

        string sceneHover = GetSceneHoverText();
        string text = !string.IsNullOrEmpty(m_hover) ? m_hover : (!string.IsNullOrEmpty(sceneHover) ? sceneHover : m_status);
        DrawHoverLabel(text, labelY, scale);
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
        m_labelStyle.normal.textColor = new Color(0.78f, 0.74f, 0.62f);
        m_labelStyle.fontStyle = FontStyle.Bold;
        m_labelStyle.wordWrap = false;

        m_smallStyle = new GUIStyle(m_labelStyle);
        m_smallStyle.normal.textColor = new Color(0.45f, 0.58f, 0.56f);
    }

    void DrawBarFrame(float barHeight, float border, float scale)
    {
        Color dark = new Color(0.025f, 0.035f, 0.05f, 0.96f);
        Color edge = new Color(0.20f, 0.32f, 0.31f, 1f);
        Color shadow = new Color(0f, 0f, 0f, 0.8f);

        Rect full = new Rect(0, 0, Screen.width, barHeight);
        Fill(full, dark);
        Fill(new Rect(0, barHeight - border, Screen.width, border), edge);
        Fill(new Rect(0, barHeight, Screen.width, Mathf.Round(5f * scale)), shadow);

        float notchW = Mathf.Round(112f * scale);
        float notchH = Mathf.Round(16f * scale);
        float notchX = Mathf.Round((Screen.width - notchW) * 0.5f);
        Fill(new Rect(notchX, barHeight - border, notchW, notchH), edge);
        Fill(new Rect(notchX + border, barHeight - border, notchW - border * 2f, notchH - border), dark);
        Fill(new Rect(0, 0, Screen.width, border), new Color(0.01f, 0.014f, 0.02f, 1f));
    }

    void DrawButton(Rect rect, ToolButton button, float scale, Vector2 mouse, Event evt)
    {
        bool over = rect.Contains(mouse);
        bool active = (button.Mode == s_mode && button.Kind != ToolButtonKind.File && button.Kind != ToolButtonKind.Card && button.Kind != ToolButtonKind.Gear);

        Fill(rect, active ? new Color(0.08f, 0.14f, 0.14f, 1f) : new Color(0.035f, 0.045f, 0.065f, 1f));
        Fill(new Rect(rect.x, rect.y, rect.width, Mathf.Max(2f, 2f * scale)), new Color(0.25f, 0.38f, 0.35f, over ? 1f : 0.65f));
        Fill(new Rect(rect.x, rect.yMax - Mathf.Max(3f, 3f * scale), rect.width, Mathf.Max(3f, 3f * scale)), new Color(0.02f, 0.025f, 0.035f, 1f));

        Color icon = over || active ? new Color(0.78f, 0.76f, 0.64f, 1f) : new Color(0.31f, 0.45f, 0.43f, 1f);
        DrawIcon(rect, button.Kind, icon, scale);

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

    void Activate(ToolButton button)
    {
        if (button.Kind == ToolButtonKind.File)
        {
            G.InventoryBar.Show();
            m_status = "Evidence drawer";
            return;
        }

        if (button.Kind == ToolButtonKind.Card)
        {
            GuiSave.Script.ShowSave();
            m_status = "Save game";
            return;
        }

        if (button.Kind == ToolButtonKind.Gear)
        {
            G.Options.Show();
            m_status = "Options";
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

    void DrawHoverLabel(string text, float y, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return;

        m_labelStyle.fontSize = Mathf.RoundToInt(22f * scale);
        Rect shadow = new Rect(0, y + Mathf.Round(2f * scale), Screen.width, Mathf.Round(32f * scale));
        Rect rect = new Rect(0, y, Screen.width, Mathf.Round(32f * scale));
        Color old = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.8f);
        GUI.Label(shadow, text, m_labelStyle);
        GUI.color = Color.white;
        GUI.Label(rect, text, m_labelStyle);
        GUI.color = old;
    }

    void DrawIcon(Rect r, ToolButtonKind kind, Color c, float scale)
    {
        float u = Mathf.Max(2f, Mathf.Round(3f * scale));
        Rect inner = new Rect(r.x + r.width * 0.22f, r.y + r.height * 0.18f, r.width * 0.56f, r.height * 0.62f);

        switch (kind)
        {
            case ToolButtonKind.Hand:
                Fill(new Rect(inner.x + u * 2, inner.y + u * 6, inner.width - u * 4, u * 4), c);
                for (int i = 0; i < 4; i++)
                    Fill(new Rect(inner.x + u * (1 + i * 2), inner.y + u * (1 + i % 2), u * 2, u * 8), c);
                Fill(new Rect(inner.x, inner.y + u * 8, u * 4, u * 3), c);
                break;
            case ToolButtonKind.Eye:
                Fill(new Rect(inner.x + u * 2, inner.y + u * 4, inner.width - u * 4, u * 3), c);
                Fill(new Rect(inner.x, inner.y + u * 6, inner.width, u * 5), c);
                Fill(new Rect(inner.x + inner.width * 0.42f, inner.y + u * 5, u * 4, u * 6), Color.black);
                break;
            case ToolButtonKind.Boot:
                Fill(new Rect(inner.x + u * 5, inner.y + u * 1, u * 5, u * 12), c);
                Fill(new Rect(inner.x + u * 2, inner.y + u * 11, u * 12, u * 4), c);
                Fill(new Rect(inner.x, inner.y + u * 13, u * 9, u * 3), c);
                break;
            case ToolButtonKind.File:
                Fill(new Rect(inner.x + u * 2, inner.y, inner.width - u * 3, inner.height), c);
                Fill(new Rect(inner.x + inner.width - u * 5, inner.y, u * 5, u * 5), Color.black);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 7, inner.width - u * 10, u * 2), Color.black);
                Fill(new Rect(inner.x + u * 5, inner.y + u * 12, inner.width - u * 12, u * 2), Color.black);
                break;
            case ToolButtonKind.Card:
                Fill(new Rect(inner.x, inner.y + u * 3, inner.width, inner.height - u * 6), c);
                Fill(new Rect(inner.x + u * 3, inner.y + u * 6, inner.width - u * 6, u * 3), Color.black);
                Fill(new Rect(inner.x + inner.width - u * 6, inner.y + inner.height - u * 9, u * 4, u * 4), Color.black);
                break;
            case ToolButtonKind.Gear:
                Fill(new Rect(inner.x + u * 5, inner.y + u * 2, u * 6, inner.height - u * 4), c);
                Fill(new Rect(inner.x + u * 2, inner.y + u * 5, inner.width - u * 4, u * 6), c);
                Fill(new Rect(inner.x + u * 7, inner.y + u * 7, u * 3, u * 3), Color.black);
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
        Hand,
        Eye,
        Boot,
        File,
        Card,
        Gear
    }
}
