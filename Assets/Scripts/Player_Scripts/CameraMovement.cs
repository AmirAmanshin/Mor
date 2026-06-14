using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public enum RotationAxes
    {
        XandY,
        X,
        Y
    }

    public RotationAxes _axes = RotationAxes.XandY;

    [Header("Input Action")]
    [SerializeField] private InputActionReference lookAction;

    public float _rotationSpeedHor = 0.2f;
    public float _rotationSpeedVer = 0.2f;

    public float maxVert = 45.0f;
    public float minVert = -45.0f;

    private float _rotationX = 0;

    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }

    private void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null) body.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        if (_axes == RotationAxes.XandY)
        {
            _rotationX -= lookInput.y * _rotationSpeedVer;
            _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert);

            float delta = lookInput.x * _rotationSpeedHor;
            float _rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }
        else if (_axes == RotationAxes.X)
        {
            transform.Rotate(0, lookInput.x * _rotationSpeedHor, 0);
        }
        else if (_axes == RotationAxes.Y)
        {
            _rotationX -= lookInput.y * _rotationSpeedVer;
            _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert);

            float _rotationY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
        }
    }
}