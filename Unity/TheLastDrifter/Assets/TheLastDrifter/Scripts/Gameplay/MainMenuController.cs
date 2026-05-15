using TheLastDrifter.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheLastDrifter.Gameplay
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private DrifterSaveSystem saveSystem;
        [SerializeField] private string firstSceneName = "Chapter01_Alley";
        [SerializeField] private string fallbackContinueSceneName = "Chapter01_Alley";

        private void Reset()
        {
            saveSystem = FindFirstObjectByType<DrifterSaveSystem>();
        }

        public void StartGame()
        {
            if (saveSystem != null) saveSystem.ResetProgress();
            SceneManager.LoadScene(firstSceneName);
        }

        public void Continue()
        {
            SceneManager.LoadScene(ResolveContinueScene());
        }

        public void ResetProgress()
        {
            if (saveSystem != null) saveSystem.ResetProgress();
        }

        private string ResolveContinueScene()
        {
            if (saveSystem == null || saveSystem.State == null)
                return fallbackContinueSceneName;

            return saveSystem.State.CurrentSceneId switch
            {
                DrifterIds.BloodLab => "Chapter02_BloodLab",
                DrifterIds.Zoo => "Chapter03_Zoo",
                _ => fallbackContinueSceneName
            };
        }
    }
}
