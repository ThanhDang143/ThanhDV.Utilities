using System.Linq;
using System.Threading.Tasks;
using ThanhDV.Utilities.DebugExtensions;
using ThanhDV.Utilities.RectTransformExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhDV.Utilities.UIAdaptation
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class UIBackground : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool setupOnAwake = true;

        private bool isSetupComplete;

        private void Awake()
        {
            isSetupComplete = false;
        }

        private void Start()
        {
            if (setupOnAwake) Setup();
        }

        public void Setup()
        {
            if (!transform.parent.TryGetComponent<RectTransform>(out var parentRectTransform)) parentRectTransform = transform.parent.GetComponentInParent<RectTransform>();
            if (parentRectTransform == null)
            {
                Debug.Log("<color=red>Parent RectTransform not found!!!</color>");
                return;
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.SetAnchor(AnchorPresets.MiddleCenter);
            Image image = GetComponent<Image>();
            Vector3 imageSize = image.sprite.bounds.size;
            Vector2 bgSize = Vector2.zero;

            bgSize.x = parentRectTransform.rect.width;
            bgSize.y = bgSize.x * imageSize.y / imageSize.x;

            if (bgSize.y < parentRectTransform.rect.height)
            {
                bgSize.y = parentRectTransform.rect.height;
                bgSize.x = bgSize.y * imageSize.x / imageSize.y;
            }

            rectTransform.sizeDelta = bgSize;
            isSetupComplete = true;
        }

        public Vector2 GetSize()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            return rectTransform.sizeDelta;
        }

        public async Task<Vector2> GetSetupSize(float timeout = 1f)
        {
            while (!isSetupComplete && timeout > 0)
            {
                await Task.Yield();
            }

            if (!isSetupComplete) Debug.Log("<color=red>Background was not setup!</color>");
            return GetComponent<RectTransform>().sizeDelta;
        }
    }
}
