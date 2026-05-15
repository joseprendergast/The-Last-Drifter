using TheLastDrifter.UI;
using UnityEngine;

namespace TheLastDrifter.Gameplay
{
    public sealed class PointClickInput : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;
        [SerializeField] private NoirSubtitlePresenter subtitles;
        [SerializeField] private LayerMask hotspotLayers = ~0;
        [SerializeField] private float rayDistance = 100f;

        private HotspotController hovered;

        private void Reset()
        {
            sceneCamera = Camera.main;
            subtitles = FindFirstObjectByType<NoirSubtitlePresenter>();
        }

        private void Awake()
        {
            if (sceneCamera == null) sceneCamera = Camera.main;
            if (subtitles == null) subtitles = FindFirstObjectByType<NoirSubtitlePresenter>();
        }

        private void Update()
        {
            var current = GetHotspotUnderPointer();
            if (current != hovered)
            {
                hovered = current;
                if (hovered != null && subtitles != null)
                    subtitles.ShowLine("", hovered.Label);
            }

            if (hovered != null && Input.GetMouseButtonDown(0))
                hovered.Activate();
        }

        private HotspotController GetHotspotUnderPointer()
        {
            if (sceneCamera == null) return null;

            var ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, rayDistance, hotspotLayers))
                return null;

            return hit.collider.GetComponentInParent<HotspotController>();
        }
    }
}
