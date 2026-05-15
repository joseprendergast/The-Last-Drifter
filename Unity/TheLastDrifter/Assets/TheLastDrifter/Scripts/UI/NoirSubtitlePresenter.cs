using System.Collections;
using System.Reflection;
using UnityEngine;

namespace TheLastDrifter.UI
{
    public sealed class NoirSubtitlePresenter : MonoBehaviour
    {
        [SerializeField] private Object speakerText;
        [SerializeField] private Object lineText;
        [SerializeField] private float charactersPerSecond = 42f;

        private Coroutine typing;

        public void ShowLine(string speaker, string line)
        {
            if (typing != null) StopCoroutine(typing);
            typing = StartCoroutine(TypeLine(speaker, line));
        }

        public void Clear()
        {
            if (typing != null) StopCoroutine(typing);
            typing = null;
            SetText(speakerText, "");
            SetText(lineText, "");
        }

        private IEnumerator TypeLine(string speaker, string line)
        {
            SetText(speakerText, speaker ?? "");
            if (lineText == null) yield break;

            line ??= "";
            SetText(lineText, "");
            var wait = new WaitForSeconds(1f / Mathf.Max(1f, charactersPerSecond));
            for (var i = 0; i < line.Length; i++)
            {
                SetText(lineText, line[..(i + 1)]);
                yield return wait;
            }
            typing = null;
        }

        private static void SetText(Object target, string value)
        {
            if (target == null) return;
            var property = target.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
            property?.SetValue(target, value);
        }
    }
}
