using UnityEngine;

namespace TheLastDrifter.Data
{
    [CreateAssetMenu(menuName = "The Last Drifter/Hotspot Definition")]
    public sealed class HotspotDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
        [SerializeField] private string requiredFlag;
        [SerializeField] private string flagToSet;
        [SerializeField] private EvidenceDefinition evidenceToAdd;
        [TextArea(2, 6)]
        [SerializeField] private string responseLine;
        [SerializeField] private string responseSpeaker;

        public string Id => id;
        public string Label => label;
        public string RequiredFlag => requiredFlag;
        public string FlagToSet => flagToSet;
        public EvidenceDefinition EvidenceToAdd => evidenceToAdd;
        public string ResponseLine => responseLine;
        public string ResponseSpeaker => responseSpeaker;
    }
}

