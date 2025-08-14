using UnityEngine;

namespace ThanhDV.Utilities
{
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

                        Debug.Log($"<color=yellow>{_instance.GetType().Name} instance is null!!! Auto create new instance!!!</color>");
                    }
                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;
    }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
                        _instance = FindFirstObjectByType(typeof(T)) as T;

                        if (_instance == null)
                        {
                            _instance = new GameObject().AddComponent<T>();
                            _instance.gameObject.name = _instance.GetType().Name;

                            Debug.Log($"<color=yellow>{_instance.GetType().Name} instance is null!!! Auto create new instance!!!</color>");
                        }
                    }
                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;

        // Check for case have many instance of T
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                return;
            }

            Destroy(gameObject);
        }
    }

    public class PersistentMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
                        _instance = FindFirstObjectByType(typeof(T)) as T;

                        if (_instance == null)
                        {
                            _instance = new GameObject().AddComponent<T>();
                            _instance.gameObject.name = _instance.GetType().Name;

                            Debug.Log($"<color=yellow>{_instance.GetType().Name} instance is null!!! Auto create new instance!!!</color>");
                        }
                        DontDestroyOnLoad(_instance);
                    }
                    return _instance;
                }
            }
        }

        public static bool IsExist => _instance != null;

        // Check for case have many instance of T
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(_instance);
                return;
            }

            Destroy(gameObject);
        }
    }
}