using ThanhDV.Utilities.DebugExtensions;
using UnityEngine;

namespace ThanhDV.Utilities.UIAdaptation
{
    public class PersistentOrthographicCamera : MonoBehaviour
    {
        [Space]
        [SerializeField] private bool activateOnAwake = true;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080, 1920);

        private void Awake()
        {
            if (activateOnAwake)
            {
                ResizeCamera();
            }
        }

        private void ResizeCamera()
        {
            if (mainCamera == null)
            {
                Debug.Log("<color=red>Main camera is null, trying to find the main camera...</color>");
                mainCamera = Camera.main;
            }

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float baseHorizontalSize = mainCamera.orthographicSize * referenceResolution.x / referenceResolution.y;
            float size = baseHorizontalSize * screenHeight / screenWidth;

            mainCamera.orthographicSize = size;
        }
    }
}
