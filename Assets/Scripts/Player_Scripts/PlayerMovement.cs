using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 2.5f;

    
    private CharacterController _characterController;
    private PlayerStats _playerStats;

    public bool isGrounded => _characterController.isGrounded;
    public float CurrentGravity => Physics.gravity.y * gravityMultiplier;

    private float _verticalVelocity;
    private float currentSpeed;

    private void ApplyGravity()
    {
        if (isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f;

        _verticalVelocity += CurrentGravity * Time.deltaTime;
    }

    private void PerformJump()
        {
            _verticalVelocity = _playerStats.jumpForce;
        }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
    }



    void Update()
    {
        // Спринт.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = _playerStats.speed * 2;
        }
        else
        {
            currentSpeed = _playerStats.speed;
        }

        float deltaX = Input.GetAxis("Horizontal") * currentSpeed;
        float deltaZ = Input.GetAxis("Vertical") * currentSpeed;

        Vector3 horizontalMove = transform.TransformDirection(new Vector3(deltaX, 0, deltaZ));
        horizontalMove = Vector3.ClampMagnitude(horizontalMove, currentSpeed);

        ApplyGravity();

        // Прыжок.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PerformJump();
        }

        Vector3 finalMove = (horizontalMove + Vector3.up * _verticalVelocity) * Time.deltaTime;
        _characterController.Move(finalMove);
    }
}