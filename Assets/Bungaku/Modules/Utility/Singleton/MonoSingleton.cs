using UnityEngine;

namespace Bungaku.Utility
{
    /// <summary>
    /// Generic singleton base class for global, single-instance MonoBehaviors.
    /// </summary>
    /// <typeparam name="T">The class type to define</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Storage for the singleton instance.
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Property that either creates or returns the existing singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                _instance = FindObjectOfType<T>();
                if (_instance != null)
                    return _instance;

                _instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                _instance.transform.parent = null; // suppress warning message when singleton is a child game object
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}
