using TheLastDrifter.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheLastDrifter.Gameplay
{
    public sealed class SceneExitController : MonoBehaviour
    {
        [SerializeField] private DrifterSaveSystem saveSystem;
        [SerializeField] private string completedSceneId;
        [SerializeField] private string nextSceneName;

        private void Reset()
        {
            saveSystem = FindFirstObjectByType<DrifterSaveSystem>();
        }

        public void ExitScene()
        {
            if (saveSystem != null && !string.IsNullOrWhiteSpace(completedSceneId))
                saveSystem.CompleteScene(completedSceneId);

            if (!string.IsNullOrWhiteSpace(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }
}
