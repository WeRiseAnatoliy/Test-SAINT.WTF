using UnityEngine;

namespace TestTask.Game.UI
{
    public class PinnedUIPanel : MonoBehaviour
    {
        public Transform Target;
        public Vector2 Offset;

        private Camera mCamera;

        private void Awake()
        {
            mCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            if(Target && mCamera)
            {
                var pos = RectTransformUtility.WorldToScreenPoint(mCamera, Target.position);
                GetComponent<RectTransform>().position = pos;
            }
        }
    }
}