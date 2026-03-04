using UnityEngine;

namespace ThanhDV.Utilities
{
    /// <summary>
    /// Singleton for pure C# classes (Non-MonoBehaviour).
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        Debug.Log($"<color=yellow>[Singleton] {_instance.GetType().Name} created!</color>");
                    }
                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;
    }

    /// <summary>
    /// Singleton for MonoBehaviour. Safely handles initialization and destruction.
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.Log($"<color=yellow>[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType(typeof(T)) as T;

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).Name;

                            Debug.Log($"<color=yellow>[Singleton] {typeof(T).Name} instance created!</color>");
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.Log($"<color=yellow>[Singleton] Another instance of {typeof(T)} detected! Destroying new one.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }
    }

    /// <summary>
    /// A Singleton that persists across Scenes (DontDestroyOnLoad).
    /// </summary>
    public class PersistentMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.Log($"<color=yellow>[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType(typeof(T)) as T;

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).Name;

                            Debug.Log($"<color=yellow>[Singleton] {typeof(T).Name} instance created!</color>");
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.Log($"<color=yellow>[Singleton] Another instance of {typeof(T)} detected! Destroying new one.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }
    }
}