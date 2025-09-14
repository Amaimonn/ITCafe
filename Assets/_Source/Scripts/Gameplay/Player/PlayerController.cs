using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _moveSpeed = 20f;
    [SerializeField, Min(0f)] private float _rotationSpeed = 0.1f;
    [SerializeField] private Transform _verticalRotationTransform;
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _rotationAction;

    private Vector2 _moveInput;
    // private Vector2 _rotationInput;
    // private float _pitch;

    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    #region MonoBeahviour
    private void Update()
    {
        _moveInput = _moveAction.action.ReadValue<Vector2>();
        // _rotationInput = _rotationAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move(_moveInput * Time.fixedDeltaTime);
        
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
    // private void RotateToTarget()
    // {
    //     // Поворачиваемся в направлении камеры
    //     var cameraProjection = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
    //     var cameraRotation = Quaternion.LookRotation(cameraProjection);
    //     _rigidbody.MoveRotation(cameraRotation);
    // }

    private void RotateCamera(Vector2 rotationInput)
    {
        _verticalRotationTransform.Rotate(Vector3.right, -rotationInput.y * _rotationSpeed);

        // _pitch -= rotationInput.y * _rotationSpeed;
        // _pitch = Mathf.Clamp(_pitch, -80, 80);
        // Ограничиваем вращение по вертикали
        // _verticalRotationTransform.localEulerAngles = new Vector3(_pitch, _verticalRotationTransform.localEulerAngles.y, 0);

        // Получаем нормализованный угол (-180 до 180)
        var currentAngleX = NormalizeAngle(_verticalRotationTransform.localEulerAngles.x);

        // Ограничиваем вращение по вертикали
        if (currentAngleX > 80f)
            _verticalRotationTransform.localEulerAngles = new Vector3(80f, 0, 0);
        else if (currentAngleX < -80f)
            _verticalRotationTransform.localEulerAngles = new Vector3(-80f, 0, 0);
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

}
