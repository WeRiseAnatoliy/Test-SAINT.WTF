using System.Collections.Generic;
using UnityEngine;

namespace TestTask.Triggers
{
    public class ObjectTrigger : MonoBehaviour
    {
        public string TargetTag;
        public System.Action<GameObject> OnDetected;
        public System.Action<GameObject> OnLost;

        public List<GameObject> Memory = new();

        private void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(TargetTag) || other.tag == TargetTag)
            {
                Memory.Add(other.gameObject);
                OnDetected?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(TargetTag) || other.tag == TargetTag)
            {
                Memory.Remove(other.gameObject);
                OnLost?.Invoke(other.gameObject);
            }
        }
    }
}