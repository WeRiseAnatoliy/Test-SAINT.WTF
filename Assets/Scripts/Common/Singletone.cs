using UnityEngine;

namespace TestTask.Common
{
    public class Singletone<T> : MonoBehaviour where T : MonoBehaviour //This is only for test task. I usually don't use this
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            Instance = this as T;
        }
    }
}