using UnityEngine;

namespace WaterRipples
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static bool Exists => instance != null; 
        public static T Instance => Exists ? instance : null;

        protected virtual void Awake()
        {
            instance = this as T;
        }

        protected virtual void Start() { }
        protected virtual void OnDestroy() { }
        protected virtual void Update() { }
    }
}