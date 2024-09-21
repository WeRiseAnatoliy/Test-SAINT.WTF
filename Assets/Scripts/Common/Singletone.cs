using UnityEngine;

namespace TestTask.Common
{
    public class Singletone<T> : MonoBehaviour where T : MonoBehaviour //This is only for test task. I usually don't use this
    {
        private static T _instance;

        public static T Instance 
        { 
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                return _instance;
            }

            private set => _instance = value;
        }

        private void Awake()
        {
            _instance = this as T;
        }
    }
}