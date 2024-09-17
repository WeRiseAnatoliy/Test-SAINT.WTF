using UnityEngine;

namespace TestTask.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleCharacterController : MonoBehaviour
    {
        private CharacterController controller;

        public float SpeedMove = 1f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private Vector2 moveAxis =>
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        private Vector3 moveDirection =>
            Vector3.right * moveAxis.x + Vector3.forward * moveAxis.y;

        private void Update()
        {
            var lookDirection = new Vector3(Mathf.Tan(moveAxis.x), 0, Mathf.Cos(moveAxis.y));
            transform.rotation = Quaternion.LookRotation(lookDirection);

            Move(moveDirection * SpeedMove);
            Move(Vector3.up * Physics.gravity.y);
        }

        private void Move (Vector3 direction)
        {
            controller.Move(direction);
        }
    }
}