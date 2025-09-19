using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 95f;
        [SerializeField] private Camera _camera;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private InputActionReference _moveAction;

        private Vector2 _moveInput;

        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        #region MonoBeahviour
        private void Update()
        {
            _moveInput = _moveAction.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            Move(_moveInput * Time.fixedDeltaTime);
            RotateToLook();

            _rigidbody.angularVelocity = Vector3.zero;
        }
        #endregion

        /// <summary>
        /// Передвижение вперед/назад и вправо/влево
        /// </summary>
        private void Move(Vector2 moveInput)
        {
            var forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;
            var currentSpeed = moveInput.x * _moveSpeed * right + moveInput.y * _moveSpeed * forward;
            _rigidbody.AddForce(currentSpeed, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Поворот по горизонтали и вертикали
        /// </summary>
        private void RotateToLook()
        {
            // Поворачиваемся в направлении камеры
            var cameraProjection = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            var cameraRotation = Quaternion.LookRotation(cameraProjection);
            _rigidbody.MoveRotation(cameraRotation);
        }
    }
}
