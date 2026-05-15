using UnityEngine;

namespace TheLastDrifter.Data
{
    [CreateAssetMenu(menuName = "The Last Drifter/Evidence")]
    public sealed class EvidenceDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string title;
        [SerializeField] private string sceneName;
        [TextArea(3, 8)]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        public string Id => id;
        public string Title => title;
        public string SceneName => sceneName;
        public string Description => description;
        public Sprite Icon => icon;
    }
}

