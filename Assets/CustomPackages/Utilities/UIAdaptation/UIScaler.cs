using UnityEngine;
using UnityEngine.UI;

namespace ThanhDV.Utilities.UIAdaptation
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UIScaler : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool setupOnAwake = true;

        [Space]
        [SerializeField] private float baseWidth = 1080f;
        [SerializeField] private float baseHeight = 1920f;

        private void Awake()
        {
            if (setupOnAwake) Setup();
        }

        public void Setup()
        {
            if (!TryGetComponent(out CanvasScaler scaler))
            {
                Debug.Log("<color=red>CanvasScaler not found!!!</color>");
                return;
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(baseWidth, baseHeight);

            float w = baseWidth / Screen.width;
            float h = baseHeight / Screen.height;
            float ratio = h / w;
            ratio = ratio >= 1 ? 1 : 0;
            scaler.matchWidthOrHeight = ratio;
        }
    }
}
