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
                ResizeCamera(out _);
            }
        }

        public void ResizeCamera(out float _orthographicSize)
        {
            if (mainCamera == null)
            {
                Debug.Log("<color=red>Main camera is null!!! Trying to find the main camera...</color>");
                mainCamera = Camera.main;
            }

            if (mainCamera == null)
            {
                Debug.Log("<color=red>Main camera is null!!! Can not find mainCamera!!!</color>");
                _orthographicSize = -1f;
                return;
            }

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float baseHorizontalSize = mainCamera.orthographicSize * referenceResolution.x / referenceResolution.y;
            _orthographicSize = baseHorizontalSize * screenHeight / screenWidth;

            mainCamera.orthographicSize = _orthographicSize;
        }
    }
}
