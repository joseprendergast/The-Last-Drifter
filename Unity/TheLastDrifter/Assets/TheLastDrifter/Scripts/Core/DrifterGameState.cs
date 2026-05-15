using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastDrifter.Core
{
    [Serializable]
    public sealed class DrifterGameState
    {
        [SerializeField] private string currentSceneId = "";
        [SerializeField] private List<string> flags = new();
        [SerializeField] private List<string> evidence = new();
        [SerializeField] private List<string> completedScenes = new();

        public string CurrentSceneId
        {
            get => currentSceneId;
            set => currentSceneId = value ?? "";
        }

        public IReadOnlyList<string> Evidence => evidence;
        public IReadOnlyList<string> CompletedScenes => completedScenes;

        public bool HasFlag(string id) => !string.IsNullOrWhiteSpace(id) && flags.Contains(id);

        public bool SetFlag(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || flags.Contains(id)) return false;
            flags.Add(id);
            return true;
        }

        public bool AddEvidence(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || evidence.Contains(id)) return false;
            evidence.Add(id);
            return true;
        }

        public bool CompleteScene(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || completedScenes.Contains(id)) return false;
            completedScenes.Add(id);
            return true;
        }

        public void Reset()
        {
            currentSceneId = "";
            flags.Clear();
            evidence.Clear();
            completedScenes.Clear();
        }
    }
}

