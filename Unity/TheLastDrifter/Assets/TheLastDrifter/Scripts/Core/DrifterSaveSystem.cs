using System;
using System.IO;
using UnityEngine;

namespace TheLastDrifter.Core
{
    public sealed class DrifterSaveSystem : MonoBehaviour
    {
        private const string SaveFileName = "the-last-drifter-save.json";

        [SerializeField] private bool loadOnAwake = true;

        public DrifterGameState State { get; private set; } = new();
        public event Action<DrifterGameState> StateChanged;

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        private void Awake()
        {
            if (loadOnAwake) Load();
        }

        public void Load()
        {
            if (!File.Exists(SavePath))
            {
                State = new DrifterGameState();
                StateChanged?.Invoke(State);
                return;
            }

            var json = File.ReadAllText(SavePath);
            State = JsonUtility.FromJson<DrifterGameState>(json) ?? new DrifterGameState();
            StateChanged?.Invoke(State);
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
            File.WriteAllText(SavePath, JsonUtility.ToJson(State, true));
            StateChanged?.Invoke(State);
        }

        public void ResetProgress()
        {
            State.Reset();
            if (File.Exists(SavePath)) File.Delete(SavePath);
            StateChanged?.Invoke(State);
        }

        public bool SetFlag(string id)
        {
            var changed = State.SetFlag(id);
            if (changed) Save();
            return changed;
        }

        public bool AddEvidence(string id)
        {
            var changed = State.AddEvidence(id);
            if (changed) Save();
            return changed;
        }

        public bool CompleteScene(string id)
        {
            var changed = State.CompleteScene(id);
            if (changed) Save();
            return changed;
        }
    }
}

