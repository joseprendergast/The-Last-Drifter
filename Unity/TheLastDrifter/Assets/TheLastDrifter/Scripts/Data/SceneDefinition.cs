using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheLastDrifter.Data
{
    [CreateAssetMenu(menuName = "The Last Drifter/Scene Definition")]
    public sealed class SceneDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string chapter;
        [SerializeField] private string title;
        [TextArea(2, 5)]
        [SerializeField] private string description;
        [SerializeField] private string unitySceneName;
        [SerializeField] private List<NarrativeBeat> openingBeats = new();

        public string Id => id;
        public string Chapter => chapter;
        public string Title => title;
        public string Description => description;
        public string UnitySceneName => unitySceneName;
        public IReadOnlyList<NarrativeBeat> OpeningBeats => openingBeats;
    }

    [Serializable]
    public sealed class NarrativeBeat
    {
        public string speaker;
        [TextArea(2, 5)]
        public string line;
        public float holdSeconds = 0.5f;
    }
}

