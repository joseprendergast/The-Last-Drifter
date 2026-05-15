using System.Collections.Generic;
using TheLastDrifter.Core;
using TheLastDrifter.Data;
using TheLastDrifter.Gameplay;
using TheLastDrifter.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheLastDrifter.Editor
{
    public static class DrifterSceneGenerator
    {
        private const string Root = "Assets/TheLastDrifter";
        private const string ScenesRoot = Root + "/Scenes";
        private const string DataRoot = Root + "/ScriptableObjects";
        private const string MaterialsRoot = Root + "/Materials";

        private static readonly List<string> Report = new();
        private static Material asphalt;
        private static Material ink;
        private static Material fog;
        private static Material blood;
        private static Material amber;
        private static Material green;
        private static Material paper;
        private static Material blue;

        [MenuItem("The Last Drifter/Generate Playable Slice Scenes")]
        public static void GeneratePlayableSliceScenes()
        {
            Report.Clear();
            EnsureFolders();
            EnsureMaterials();

            var data = BuildStoryData();
            CreateMainMenu();
            CreateAlley(data);
            CreateLab(data);
            CreateZoo(data);
            SetBuildScenes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[The Last Drifter] Generated playable slice scenes:\n" + string.Join("\n", Report));
        }

        private static void EnsureFolders()
        {
            System.IO.Directory.CreateDirectory(ScenesRoot + "/MainMenu");
            System.IO.Directory.CreateDirectory(ScenesRoot + "/Chapter01_Alley");
            System.IO.Directory.CreateDirectory(ScenesRoot + "/Chapter02_BloodLab");
            System.IO.Directory.CreateDirectory(ScenesRoot + "/Chapter03_Zoo");
            System.IO.Directory.CreateDirectory(DataRoot + "/Scenes");
            System.IO.Directory.CreateDirectory(DataRoot + "/Hotspots");
            System.IO.Directory.CreateDirectory(DataRoot + "/Evidence");
            System.IO.Directory.CreateDirectory(MaterialsRoot);
            AssetDatabase.Refresh();
        }

        private static void EnsureMaterials()
        {
            asphalt = Material("Noir_Wet_Asphalt", new Color(0.025f, 0.028f, 0.032f, 1f));
            ink = Material("Noir_Ink_Black", new Color(0.005f, 0.006f, 0.008f, 1f));
            fog = Material("Noir_Rain_Fog", new Color(0.17f, 0.2f, 0.22f, 0.82f));
            blood = Material("Noir_Blood", new Color(0.45f, 0.015f, 0.018f, 1f));
            amber = Material("Noir_Amber_Light", new Color(1f, 0.55f, 0.16f, 1f));
            green = Material("Noir_Sick_Green", new Color(0.12f, 0.45f, 0.25f, 1f));
            paper = Material("Noir_Paper", new Color(0.72f, 0.68f, 0.55f, 1f));
            blue = Material("Noir_Monitor_Blue", new Color(0.05f, 0.22f, 0.42f, 1f));
        }

        private static StoryData BuildStoryData()
        {
            var hand = Evidence("severed_hand_clue", "Severed hand", "Rain Alley",
                "A hand on wet tile, posed like it was reaching for the detective coat. The cut is too clean for panic.");
            var note = Evidence("stained_note", "Stained note", "Blood Lab",
                "A note half-dissolved in blood: 'GABARDINA REMEMBERS BACKWARDS.'");
            var ticket = Evidence("zoo_ticket", "Zoo ticket", "Morning Zoo",
                "A family ticket stamped with tomorrow's date. Someone wanted this memory to arrive early.");
            var reel = Evidence("memory_reel", "Memory reel", "Blood Lab",
                "A recording spool from the sub-level. The audio is mostly rain, then a child whispering from too close to the mic.");

            return new StoryData
            {
                Alley = SceneDef("alley", "Chapter 01", "Rain Alley",
                    "Captain Gabardina enters the rain and finds the first impossible hand.", "Chapter01_Alley",
                    ("GABARDINA", "Rain makes every street honest. Tonight, it only makes the blood harder to read.", 4f),
                    ("", "The coat is already on the floor. One hand waits beside it.", 3f)),
                Lab = SceneDef("blood_lab", "Chapter 01", "Blood Lab",
                    "A sub-level lavatory hums beneath dead monitors and red water.", "Chapter02_BloodLab",
                    ("GABARDINA", "The lab is under the city, but the rain still finds its way down.", 4f),
                    ("", "The lavatory breathes through the drains.", 3f)),
                Zoo = SceneDef("zoo", "Chapter 01", "Morning Zoo",
                    "A public morning breaks apart under a sign that begins to bleed.", "Chapter03_Zoo",
                    ("", "Morning. Families. Paper cups. The lie of normal life.", 3.5f),
                    ("CHILD", "Don't look at the sign when it starts remembering.", 3.5f)),
                InspectHand = Hotspot("inspect_hand", "Inspect hand", DrifterIds.SawHand, hand, "GABARDINA", "The hand is cold, but the ring mark is fresh."),
                CheckDrain = Hotspot("check_drain", "Check drain", DrifterIds.CheckedDrain, null, "GABARDINA", "Rainwater runs uphill here. No drain should do that."),
                InspectCoat = Hotspot("inspect_coat", "Inspect coat", "found_gabardina_coat", null, "GABARDINA", "My coat. My size. My blood type."),
                AlleyExit = Hotspot("enter_lab", "Follow blood trail", "entered_lab", null, "", "The alley folds into fluorescent red.", DrifterIds.SawHand),
                RestorePower = Hotspot("restore_power", "Restore power", DrifterIds.LabPowerRestored, null, "GABARDINA", "The monitors wake up one at a time, like eyes pretending to be machines."),
                ReadNote = Hotspot("read_stained_note", "Read stained note", "found_stained_note", note, "", "The handwriting is mine. The warning is not."),
                TakeReel = Hotspot("take_memory_reel", "Take memory reel", DrifterIds.RecoveredMemoryReel, reel, "CHILD", "You already heard this before you were born.", DrifterIds.LabPowerRestored),
                LabExit = Hotspot("surface_to_zoo", "Surface", "entered_zoo", null, "", "A morning crowd replaces the alarms.", DrifterIds.RecoveredMemoryReel),
                InspectSign = Hotspot("inspect_bleeding_sign", "Inspect bleeding sign", DrifterIds.SawBleedingSign, null, "GABARDINA", "The zoo map bleeds from the animal names first."),
                TalkChild = Hotspot("hear_child_warning", "Talk to child", DrifterIds.HeardChildWarning, null, "CHILD", "Captain Gabardina, the missing hand is not missing. It is choosing."),
                TakeTicket = Hotspot("take_zoo_ticket", "Take ticket", "found_zoo_ticket", ticket, "GABARDINA", "Tomorrow's date. Today's blood. Yesterday's fear.")
            };
        }

        private static void CreateMainMenu()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cam = Camera2D("Main Camera", new Color(0.01f, 0.012f, 0.016f, 1f));
            var set = new GameObject("Title Screen Set").transform;
            Quad("Rain Curtain", new Vector3(0, 0, 1), new Vector3(18, 10, 1), fog, set);
            Quad("Black Vignette", new Vector3(0, 0, 1.1f), new Vector3(16, 9, 1), ink, set);
            Label("THE LAST DRIFTER", new Vector3(0, 1.6f, 0), 54, new Color(0.88f, 0.86f, 0.78f, 1f), set, "Title");
            var startText = Label("Start Game", new Vector3(0, 0.15f, 0), 28, Color.white, set, "Start Game Label");
            var optionText = Label("Continue    Scene Select    Settings", new Vector3(0, -0.55f, 0), 18, new Color(0.58f, 0.63f, 0.66f, 1f), set, "Menu Options Label");
            var runtime = new GameObject("Runtime");
            var save = runtime.AddComponent<DrifterSaveSystem>();
            var subs = runtime.AddComponent<NoirSubtitlePresenter>();
            var input = runtime.AddComponent<PointClickInput>();
            var menu = runtime.AddComponent<MainMenuController>();
            var subSo = new SerializedObject(subs);
            subSo.FindProperty("speakerText").objectReferenceValue = optionText.GetComponent<TextMesh>();
            subSo.FindProperty("lineText").objectReferenceValue = startText.GetComponent<TextMesh>();
            subSo.ApplyModifiedPropertiesWithoutUndo();
            var inputSo = new SerializedObject(input);
            inputSo.FindProperty("sceneCamera").objectReferenceValue = cam;
            inputSo.FindProperty("subtitles").objectReferenceValue = subs;
            inputSo.ApplyModifiedPropertiesWithoutUndo();
            var so = new SerializedObject(menu);
            so.FindProperty("saveSystem").objectReferenceValue = save;
            so.ApplyModifiedPropertiesWithoutUndo();
            var hotspots = new GameObject("Hotspots").transform;
            var startDef = Hotspot("menu_start_game", "Start Game", "", null, "", "The rain opens.", "");
            AddExit(HotspotObj("Start Game", new Vector3(0, 0.15f, -1), new Vector3(2.4f, 0.7f, 1), startDef, hotspots, save, subs), save, "", "Chapter01_Alley");
            Save(scene, ScenesRoot + "/MainMenu/MainMenu.unity", "MainMenu");
        }

        private static void CreateAlley(StoryData data)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupSceneBasics(new Color(0.018f, 0.022f, 0.028f, 1f), data.Alley, out var set, out var hotspots, out var save, out var subs);
            Quad("Wet Alley Floor", new Vector3(0, -2.6f, 0), new Vector3(14, 3.8f, 1), asphalt, set);
            Quad("Back Wall", new Vector3(0, 1.0f, 0.4f), new Vector3(14, 5.2f, 1), ink, set);
            Quad("Door Glow", new Vector3(4.8f, 0.4f, 0.2f), new Vector3(1.25f, 2.6f, 1), amber, set);
            Cube("Captain Gabardina Coat", new Vector3(-1.6f, -2.2f, -0.1f), new Vector3(1.25f, 0.25f, 0.08f), ink, set, false);
            Cube("Severed Hand Marker", new Vector3(0.3f, -2.15f, -0.1f), new Vector3(0.45f, 0.18f, 0.08f), paper, set, false);
            for (var i = 0; i < 26; i++)
                Quad("Rain Streak " + i, new Vector3(-6.5f + i * 0.52f, 0.2f + (i % 5) * 0.7f, -0.2f), new Vector3(0.025f, 1.4f, 1), fog, set);
            HotspotObj("Inspect hand", new Vector3(0.3f, -2.15f, -1), new Vector3(0.9f, 0.55f, 1), data.InspectHand, hotspots, save, subs);
            HotspotObj("Inspect coat", new Vector3(-1.6f, -2.15f, -1), new Vector3(1.5f, 0.7f, 1), data.InspectCoat, hotspots, save, subs);
            HotspotObj("Check drain", new Vector3(-4.2f, -2.55f, -1), new Vector3(1.2f, 0.45f, 1), data.CheckDrain, hotspots, save, subs);
            AddExit(HotspotObj("Follow blood trail", new Vector3(4.8f, 0.4f, -1), new Vector3(1.6f, 2.8f, 1), data.AlleyExit, hotspots, save, subs), save, DrifterIds.Alley, "Chapter02_BloodLab");
            Save(scene, ScenesRoot + "/Chapter01_Alley/Chapter01_Alley.unity", "Chapter01_Alley");
        }

        private static void CreateLab(StoryData data)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupSceneBasics(new Color(0.025f, 0.012f, 0.014f, 1f), data.Lab, out var set, out var hotspots, out var save, out var subs);
            Quad("Blood Lavatory Floor", new Vector3(0, -2.25f, 0), new Vector3(13, 3.4f, 1), blood, set);
            Quad("Sub Level Wall", new Vector3(0, 1.1f, 0.4f), new Vector3(13, 5.3f, 1), ink, set);
            for (var i = 0; i < 5; i++)
                Quad("Dead Monitor " + i, new Vector3(-4.5f + i * 1.15f, 1.9f, -0.1f), new Vector3(0.8f, 0.55f, 1), i % 2 == 0 ? blue : green, set);
            Cube("Central Drain", new Vector3(0.4f, -2.6f, -0.1f), new Vector3(0.8f, 0.16f, 0.08f), ink, set, false);
            Cube("Stained Note", new Vector3(-2.2f, -1.75f, -0.1f), new Vector3(0.65f, 0.4f, 0.05f), paper, set, false);
            Cube("Memory Reel", new Vector3(2.8f, -1.55f, -0.1f), new Vector3(0.55f, 0.55f, 0.05f), blue, set, false);
            HotspotObj("Restore power", new Vector3(-4.7f, 1.9f, -1), new Vector3(1.2f, 0.8f, 1), data.RestorePower, hotspots, save, subs);
            HotspotObj("Read stained note", new Vector3(-2.2f, -1.75f, -1), new Vector3(1.0f, 0.7f, 1), data.ReadNote, hotspots, save, subs);
            HotspotObj("Take memory reel", new Vector3(2.8f, -1.55f, -1), new Vector3(1.0f, 1.0f, 1), data.TakeReel, hotspots, save, subs);
            AddExit(HotspotObj("Surface", new Vector3(5.35f, -0.3f, -1), new Vector3(1.4f, 2.2f, 1), data.LabExit, hotspots, save, subs), save, DrifterIds.BloodLab, "Chapter03_Zoo");
            Save(scene, ScenesRoot + "/Chapter02_BloodLab/Chapter02_BloodLab.unity", "Chapter02_BloodLab");
        }

        private static void CreateZoo(StoryData data)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupSceneBasics(new Color(0.18f, 0.2f, 0.18f, 1f), data.Zoo, out var set, out var hotspots, out var save, out var subs);
            Quad("Morning Path", new Vector3(0, -2.8f, 0), new Vector3(14, 2.8f, 1), green, set);
            Quad("Zoo Sign", new Vector3(0, 1.55f, -0.1f), new Vector3(4.2f, 1.15f, 1), paper, set);
            Label("CITY ZOO", new Vector3(0, 1.58f, -0.2f), 28, Color.black, set, "Zoo Sign Text");
            Quad("Blood From Sign", new Vector3(1.15f, 0.78f, -0.2f), new Vector3(0.18f, 1.1f, 1), blood, set);
            Cube("Child Silhouette", new Vector3(-2.9f, -1.25f, -0.1f), new Vector3(0.38f, 1.1f, 0.08f), ink, set, false);
            Cube("Ticket", new Vector3(2.5f, -1.85f, -0.1f), new Vector3(0.7f, 0.34f, 0.06f), paper, set, false);
            for (var i = 0; i < 7; i++)
                Cube("Crowd Shape " + i, new Vector3(-5f + i * 1.5f, -1.55f + (i % 2) * 0.2f, -0.1f), new Vector3(0.32f, 0.9f, 0.08f), ink, set, false);
            HotspotObj("Inspect bleeding sign", new Vector3(0, 1.4f, -1), new Vector3(4.5f, 1.5f, 1), data.InspectSign, hotspots, save, subs);
            HotspotObj("Talk to child", new Vector3(-2.9f, -1.25f, -1), new Vector3(1.0f, 1.4f, 1), data.TalkChild, hotspots, save, subs);
            HotspotObj("Take ticket", new Vector3(2.5f, -1.85f, -1), new Vector3(1.0f, 0.7f, 1), data.TakeTicket, hotspots, save, subs);
            Label("END OF PLAYABLE SLICE", new Vector3(0, -4.15f, 0), 18, new Color(0.8f, 0.78f, 0.68f, 1f), set, "Slice Hook");
            Save(scene, ScenesRoot + "/Chapter03_Zoo/Chapter03_Zoo.unity", "Chapter03_Zoo");
        }

        private static void SetupSceneBasics(Color bg, SceneDefinition sceneDef, out Transform set, out Transform hotspots, out DrifterSaveSystem save, out NoirSubtitlePresenter subs)
        {
            var cam = Camera2D("Main Camera", bg);
            set = new GameObject("Set").transform;
            hotspots = new GameObject("Hotspots").transform;
            var ui = new GameObject("Subtitle Anchor").transform;
            var speaker = Label("", new Vector3(0, -4.15f, -0.5f), 16, new Color(0.75f, 0.72f, 0.62f, 1f), ui, "Subtitle Speaker");
            var line = Label("", new Vector3(0, -4.55f, -0.5f), 20, new Color(0.86f, 0.86f, 0.78f, 1f), ui, "Subtitle Line");

            var runtime = new GameObject("Runtime");
            save = runtime.AddComponent<DrifterSaveSystem>();
            subs = runtime.AddComponent<NoirSubtitlePresenter>();
            var flow = runtime.AddComponent<SceneFlowController>();
            var input = runtime.AddComponent<PointClickInput>();

            var subSo = new SerializedObject(subs);
            subSo.FindProperty("speakerText").objectReferenceValue = speaker.GetComponent<TextMesh>();
            subSo.FindProperty("lineText").objectReferenceValue = line.GetComponent<TextMesh>();
            subSo.ApplyModifiedPropertiesWithoutUndo();

            var flowSo = new SerializedObject(flow);
            flowSo.FindProperty("sceneDefinition").objectReferenceValue = sceneDef;
            flowSo.FindProperty("saveSystem").objectReferenceValue = save;
            flowSo.FindProperty("subtitles").objectReferenceValue = subs;
            flowSo.ApplyModifiedPropertiesWithoutUndo();

            var inputSo = new SerializedObject(input);
            inputSo.FindProperty("sceneCamera").objectReferenceValue = cam;
            inputSo.FindProperty("subtitles").objectReferenceValue = subs;
            inputSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddExit(GameObject go, DrifterSaveSystem save, string completedId, string nextScene)
        {
            var exit = go.AddComponent<SceneExitController>();
            var so = new SerializedObject(exit);
            so.FindProperty("saveSystem").objectReferenceValue = save;
            so.FindProperty("completedSceneId").stringValue = completedId;
            so.FindProperty("nextSceneName").stringValue = nextScene;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireHotspot(GameObject go, HotspotDefinition def, DrifterSaveSystem save, NoirSubtitlePresenter subs)
        {
            var hc = go.AddComponent<HotspotController>();
            var so = new SerializedObject(hc);
            so.FindProperty("definition").objectReferenceValue = def;
            so.FindProperty("saveSystem").objectReferenceValue = save;
            so.FindProperty("subtitles").objectReferenceValue = subs;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject HotspotObj(string name, Vector3 pos, Vector3 scale, HotspotDefinition def, Transform parent, DrifterSaveSystem save, NoirSubtitlePresenter subs)
        {
            var go = Cube(name, pos, scale, fog, parent, true);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.enabled = false;
            WireHotspot(go, def, save, subs);
            return go;
        }

        private static Camera Camera2D(string name, Color bg)
        {
            var camGo = new GameObject(name);
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0, 0, -10);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bg;
            camGo.AddComponent<AudioListener>();
            return cam;
        }

        private static GameObject Quad(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = mat;
            var collider = go.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
            return go;
        }

        private static GameObject Cube(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent, bool keepCollider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = scale;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = mat;
            if (!keepCollider)
            {
                var collider = go.GetComponent<Collider>();
                if (collider != null) Object.DestroyImmediate(collider);
            }
            return go;
        }

        private static GameObject Label(string text, Vector3 pos, int size, Color color, Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.fontSize = size;
            tm.characterSize = 0.08f;
            tm.color = color;
            return go;
        }

        private static Material Material(string name, Color color)
        {
            var path = MaterialsRoot + "/" + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                var shader = Shader.Find("Unlit/Color");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            else
            {
                var shader = Shader.Find("Unlit/Color");
                if (shader != null) mat.shader = shader;
            }
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static EvidenceDefinition Evidence(string id, string title, string scene, string desc)
        {
            var asset = LoadOrCreate<EvidenceDefinition>(DataRoot + "/Evidence/" + id + ".asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("title").stringValue = title;
            so.FindProperty("sceneName").stringValue = scene;
            so.FindProperty("description").stringValue = desc;
            so.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static HotspotDefinition Hotspot(string id, string label, string flag, EvidenceDefinition evidence, string speaker, string line, string required = "")
        {
            var asset = LoadOrCreate<HotspotDefinition>(DataRoot + "/Hotspots/" + id + ".asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("label").stringValue = label;
            so.FindProperty("requiredFlag").stringValue = required;
            so.FindProperty("flagToSet").stringValue = flag;
            so.FindProperty("evidenceToAdd").objectReferenceValue = evidence;
            so.FindProperty("responseSpeaker").stringValue = speaker;
            so.FindProperty("responseLine").stringValue = line;
            so.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static SceneDefinition SceneDef(string id, string chapter, string title, string desc, string unitySceneName, params (string speaker, string line, float hold)[] beats)
        {
            var asset = LoadOrCreate<SceneDefinition>(DataRoot + "/Scenes/" + id + ".asset");
            var so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("chapter").stringValue = chapter;
            so.FindProperty("title").stringValue = title;
            so.FindProperty("description").stringValue = desc;
            so.FindProperty("unitySceneName").stringValue = unitySceneName;
            var arr = so.FindProperty("openingBeats");
            arr.arraySize = beats.Length;
            for (var i = 0; i < beats.Length; i++)
            {
                var el = arr.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("speaker").stringValue = beats[i].speaker;
                el.FindPropertyRelative("line").stringValue = beats[i].line;
                el.FindPropertyRelative("holdSeconds").floatValue = beats[i].hold;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return asset;
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void Save(Scene scene, string path, string name)
        {
            EditorSceneManager.SaveScene(scene, path);
            Report.Add("Saved " + name);
        }

        private static void SetBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenesRoot + "/MainMenu/MainMenu.unity", true),
                new EditorBuildSettingsScene(ScenesRoot + "/Chapter01_Alley/Chapter01_Alley.unity", true),
                new EditorBuildSettingsScene(ScenesRoot + "/Chapter02_BloodLab/Chapter02_BloodLab.unity", true),
                new EditorBuildSettingsScene(ScenesRoot + "/Chapter03_Zoo/Chapter03_Zoo.unity", true)
            };
        }

        private sealed class StoryData
        {
            public SceneDefinition Alley;
            public SceneDefinition Lab;
            public SceneDefinition Zoo;
            public HotspotDefinition InspectHand;
            public HotspotDefinition CheckDrain;
            public HotspotDefinition InspectCoat;
            public HotspotDefinition AlleyExit;
            public HotspotDefinition RestorePower;
            public HotspotDefinition ReadNote;
            public HotspotDefinition TakeReel;
            public HotspotDefinition LabExit;
            public HotspotDefinition InspectSign;
            public HotspotDefinition TalkChild;
            public HotspotDefinition TakeTicket;
        }
    }
}
