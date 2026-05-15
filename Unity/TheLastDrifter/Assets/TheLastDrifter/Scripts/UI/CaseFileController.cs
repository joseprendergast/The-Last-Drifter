using System.Collections.Generic;
using System.Reflection;
using TheLastDrifter.Core;
using TheLastDrifter.Data;
using UnityEngine;

namespace TheLastDrifter.UI
{
    public sealed class CaseFileController : MonoBehaviour
    {
        [SerializeField] private DrifterSaveSystem saveSystem;
        [SerializeField] private List<EvidenceDefinition> evidenceDefinitions = new();
        [SerializeField] private Object caseFileText;

        private readonly Dictionary<string, EvidenceDefinition> evidenceById = new();

        private void Awake()
        {
            evidenceById.Clear();
            foreach (var evidence in evidenceDefinitions)
            {
                if (evidence != null && !string.IsNullOrWhiteSpace(evidence.Id))
                    evidenceById[evidence.Id] = evidence;
            }
        }

        private void OnEnable()
        {
            if (saveSystem != null) saveSystem.StateChanged += Render;
            if (saveSystem != null) Render(saveSystem.State);
        }

        private void OnDisable()
        {
            if (saveSystem != null) saveSystem.StateChanged -= Render;
        }

        public void Render(DrifterGameState state)
        {
            if (caseFileText == null || state == null) return;
            if (state.Evidence.Count == 0)
            {
                SetText("No evidence collected.");
                return;
            }

            var lines = new List<string>();
            foreach (var id in state.Evidence)
            {
                if (!evidenceById.TryGetValue(id, out var evidence)) continue;
                lines.Add($"<b>{evidence.Title}</b> / {evidence.SceneName}\n{evidence.Description}");
            }

            SetText(string.Join("\n\n", lines));
        }

        private void SetText(string value)
        {
            var property = caseFileText.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
            property?.SetValue(caseFileText, value);
        }
    }
}
