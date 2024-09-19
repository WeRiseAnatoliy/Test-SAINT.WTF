using UnityEngine;

namespace TestTask.Game
{
    public class LinearMover : MonoBehaviour
    {
        public Transform NewTarget;
        public Vector3 LocalPoint;

        public float ClearTargetThreshold = 0.01f;
        public float SpeedMove = 4;
        public float SpeedRotate = 250f;

        private void Update()
        {
            if (NewTarget)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, LocalPoint, SpeedMove * Time.deltaTime);
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, SpeedRotate * Time.deltaTime);

                if (Vector3.Distance(transform.localPosition, LocalPoint) < ClearTargetThreshold)
                    NewTarget = null;
            }
        }

        public void SetTargetPos (Vector3 localPoint, Transform nextParent)
        {
            LocalPoint = localPoint;
            NewTarget = nextParent;
            transform.parent = nextParent;
        }
    }
}