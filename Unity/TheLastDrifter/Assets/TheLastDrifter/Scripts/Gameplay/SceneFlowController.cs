using System.Collections;
using TheLastDrifter.Core;
using TheLastDrifter.Data;
using TheLastDrifter.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheLastDrifter.Gameplay
{
    public sealed class SceneFlowController : MonoBehaviour
    {
        [SerializeField] private SceneDefinition sceneDefinition;
        [SerializeField] private DrifterSaveSystem saveSystem;
        [SerializeField] private NoirSubtitlePresenter subtitles;
        [SerializeField] private float firstBeatDelay = 0.4f;

        private void Reset()
        {
            saveSystem = FindFirstObjectByType<DrifterSaveSystem>();
            subtitles = FindFirstObjectByType<NoirSubtitlePresenter>();
        }

        private void Start()
        {
            if (sceneDefinition == null) return;
            if (saveSystem != null)
            {
                saveSystem.State.CurrentSceneId = sceneDefinition.Id;
                saveSystem.Save();
            }

            StartCoroutine(PlayOpeningBeats());
        }

        public void CompleteAndLoad(SceneDefinition nextScene)
        {
            if (sceneDefinition != null && saveSystem != null)
                saveSystem.CompleteScene(sceneDefinition.Id);

            if (nextScene != null && !string.IsNullOrWhiteSpace(nextScene.UnitySceneName))
                SceneManager.LoadScene(nextScene.UnitySceneName);
        }

        private IEnumerator PlayOpeningBeats()
        {
            yield return new WaitForSeconds(firstBeatDelay);
            if (subtitles == null || sceneDefinition == null) yield break;

            foreach (var beat in sceneDefinition.OpeningBeats)
            {
                subtitles.ShowLine(beat.speaker, beat.line);
                yield return new WaitForSeconds(Mathf.Max(0.1f, beat.holdSeconds));
            }
        }
    }
}

