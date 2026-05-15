using TheLastDrifter.Core;
using UnityEngine;

namespace TheLastDrifter.Integrations
{
    public sealed class AdventureCreatorBridge : MonoBehaviour
    {
        [SerializeField] private DrifterSaveSystem saveSystem;

        public void SetDrifterFlag(string flagId)
        {
            if (saveSystem != null) saveSystem.SetFlag(flagId);
        }

        public void AddDrifterEvidence(string evidenceId)
        {
            if (saveSystem != null) saveSystem.AddEvidence(evidenceId);
        }

        public bool HasDrifterFlag(string flagId)
        {
            return saveSystem != null && saveSystem.State.HasFlag(flagId);
        }
    }
}

