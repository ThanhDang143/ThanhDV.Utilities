using UnityEngine;

namespace ThanhDV.Utilities.UIAdaptation
{
    public class UISaveZone : MonoBehaviour
    {
        private void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            if (!TryGetComponent(out RectTransform rectTransform))
            {
                Debug.Log("<color=red>[UIAdaptation] RectTransform not found!!!</color>");
                return;
            }

            Rect saveZone = Screen.safeArea;
            Vector2 anchorMin = saveZone.position;
            Vector2 anchorMax = anchorMin + saveZone.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}
