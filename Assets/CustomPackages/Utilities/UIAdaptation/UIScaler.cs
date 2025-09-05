using Unity.VisualScripting.YamlDotNet.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhDV.Utilities.UIAdaptation
{
    [RequireComponent(typeof(CanvasScaler))]
    public class UIScaler : MonoBehaviour
    {
        [Space]
        [SerializeField] private CanvasScaler scaler;

        [Space]
        [SerializeField] private float baseWidth = 1080f;
        [SerializeField] private float baseHeight = 1920f;

        private void Awake()
        {
            if (TryGetCanvasScaler())
            {
                Prepare();
                Setup();
            }
        }

        private void Setup()
        {
            if (scaler == null)
            {
                Debug.Log("<color=red>[UIAdaptation] CanvasScaler not found!!!</color>");
                return;
            }

            float referenceRatio = baseWidth / baseHeight;
            float screenRatio = (float)Screen.width / Screen.height;

            scaler.matchWidthOrHeight = (screenRatio > referenceRatio) ? 1f : 0f;
        }

        private void Prepare()
        {
            if (scaler == null)
            {
                Debug.Log("<color=red>[UIAdaptation] CanvasScaler not found!!!</color>");
                return;
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referenceResolution = new Vector2(baseWidth, baseHeight);
        }

        private bool TryGetCanvasScaler()
        {
            if (scaler != null) return true;

            TryGetComponent(out scaler);

            return scaler != null;
        }
    }
}
