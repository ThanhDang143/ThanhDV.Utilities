using UnityEngine;

namespace ThanhDV.Utilities.UIAdaptation
{
    public class UISaveZone : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool isSetupOnAwake = true;

        private void Awake()
        {
            if (isSetupOnAwake) Setup();
        }

        public void Setup()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
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
