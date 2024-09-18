using UnityEngine;

namespace TestTask.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleCharacterController : MonoBehaviour
    {
        private CharacterController controller;

        public float SpeedMove = 1f;
        public float RotationDegressDelta = 0.5f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private Vector2 moveAxis =>
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        private Vector3 moveDirection =>
            Vector3.right * moveAxis.x + Vector3.forward * moveAxis.y;

        private void FixedUpdate()
        {
            var lookDirection = new Vector3(moveAxis.x, 0, moveAxis.y);
            if(lookDirection.magnitude > 0.25f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection), RotationDegressDelta * Time.fixedDeltaTime);

            Move(moveDirection * SpeedMove * Time.fixedDeltaTime);
            Move(Vector3.up * Physics.gravity.y * Time.fixedDeltaTime);
        }

        private void Move (Vector3 direction)
        {
            controller.Move(direction);
        }
    }
}