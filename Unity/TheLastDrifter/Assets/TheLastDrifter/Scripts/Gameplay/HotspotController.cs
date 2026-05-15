using TheLastDrifter.Core;
using TheLastDrifter.Data;
using TheLastDrifter.UI;
using UnityEngine;
using UnityEngine.Events;

namespace TheLastDrifter.Gameplay
{
    public sealed class HotspotController : MonoBehaviour
    {
        [SerializeField] private HotspotDefinition definition;
        [SerializeField] private DrifterSaveSystem saveSystem;
        [SerializeField] private NoirSubtitlePresenter subtitles;
        [SerializeField] private UnityEvent onActivated;

        public string Label => definition != null ? definition.Label : gameObject.name;

        private void Reset()
        {
            saveSystem = FindFirstObjectByType<DrifterSaveSystem>();
            subtitles = FindFirstObjectByType<NoirSubtitlePresenter>();
        }

        public bool IsAvailable()
        {
            if (definition == null || saveSystem == null) return true;
            var required = definition.RequiredFlag;
            return string.IsNullOrWhiteSpace(required) || saveSystem.State.HasFlag(required);
        }

        public void Activate()
        {
            if (!IsAvailable() || definition == null) return;

            if (definition.EvidenceToAdd != null)
                saveSystem.AddEvidence(definition.EvidenceToAdd.Id);

            if (!string.IsNullOrWhiteSpace(definition.FlagToSet))
                saveSystem.SetFlag(definition.FlagToSet);

            if (subtitles != null && !string.IsNullOrWhiteSpace(definition.ResponseLine))
                subtitles.ShowLine(definition.ResponseSpeaker, definition.ResponseLine);

            onActivated?.Invoke();
            GetComponent<SceneExitController>()?.ExitScene();
        }
    }
}
