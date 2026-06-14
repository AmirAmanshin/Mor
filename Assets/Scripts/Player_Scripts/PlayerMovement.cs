using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravityMultiplier = 2.5f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sneakAction;

    [Header("Crouch Settings")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float crouchHeight = 1.1f;
    [SerializeField] private float standingHeight = 1.7f;
    [SerializeField] private float crouchSpeedModifier = 0.65f;
    [SerializeField] private float sneakSpeedModifier = 0.45f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MovementAudio walkSounds;
    [SerializeField] private MovementAudio sprintSounds;
    [SerializeField] private MovementAudio sneakSounds;
    [SerializeField] private MovementAudio crouchSounds;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField][Range(0f, 1f)] private float globalVolumeModifier = 0.5f;

    [System.Serializable]
    public struct MovementAudio
    {
        public AudioClip[] footstepClips;
        public float stepInterval;
        [Range(0f, 1f)] public float volume;
    }

    private CharacterController _characterController;
    private PlayerStats _playerStats;
    private Animator _animator;

    private static readonly int MoveXHash = Animator.StringToHash("moveX");
    private static readonly int MoveYHash = Animator.StringToHash("moveY");
    private static readonly int JumpHash = Animator.StringToHash("jump");
    private static readonly int GroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("verticalVelocity");

    public bool isGrounded => _characterController.isGrounded;
    public float CurrentGravity => Physics.gravity.y * gravityMultiplier;

    private float _verticalVelocity;
    private float _currentSpeed;
    private bool _isCrouching;
    private float _cameraStandingY;
    private float _cameraCrouchingY;
    private float _footstepTimer = 0f;
    private bool _wasGrounded;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        sneakAction.action.Enable();

        jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        sneakAction.action.Disable();

        jumpAction.action.performed -= OnJump;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();

        standingHeight = _characterController.height;

        _cameraStandingY = cameraTarget.localPosition.y;
        _cameraCrouchingY = _cameraStandingY * (crouchHeight / standingHeight);
    }

    private void Update()
    {
        HandleCrouch();

        bool isSprinting = sprintAction.action.ReadValue<float>() > 0;
        bool isSneaking = sneakAction.action.ReadValue<float>() > 0 && !isSprinting;

        if (isSprinting && _isCrouching)
            _currentSpeed = _playerStats.speed * crouchSpeedModifier * 2f;
        else if (isSprinting)
            _currentSpeed = _playerStats.speed * 2f;
        else if (isSneaking)
            _currentSpeed = _playerStats.speed * sneakSpeedModifier;
        else if (_isCrouching)
            _currentSpeed = _playerStats.speed * crouchSpeedModifier;
        else
            _currentSpeed = _playerStats.speed;

        float multiplier = isSprinting ? 2f : (_isCrouching ? crouchSpeedModifier : (isSneaking ? sneakSpeedModifier : 1f));

        HandleFootsteps();
        HandleLanding();

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        _animator.SetFloat(MoveXHash, moveInput.x * multiplier, 0.1f, Time.deltaTime);
        _animator.SetFloat(MoveYHash, moveInput.y * multiplier, 0.1f, Time.deltaTime);
        _animator.SetFloat(VerticalVelocityHash, _verticalVelocity);
        _animator.SetBool(GroundedHash, isGrounded);

        Vector3 horizontalMove = transform.TransformDirection(new Vector3(moveInput.x * _currentSpeed, 0, moveInput.y * _currentSpeed));
        horizontalMove = Vector3.ClampMagnitude(horizontalMove, _currentSpeed);

        ApplyGravity();

        _characterController.Move((horizontalMove + Vector3.up * _verticalVelocity) * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        bool wantsToCrouch = crouchAction.action.ReadValue<float>() > 0;

        if (_isCrouching && !wantsToCrouch)
        {
            Vector3 rayStart = transform.position + Vector3.up * _characterController.height;
            float rayDistance = standingHeight - crouchHeight;
            bool blocked = Physics.Raycast(rayStart, Vector3.up, rayDistance, obstacleLayer);
            Debug.DrawRay(rayStart, Vector3.up * rayDistance, blocked ? Color.red : Color.green);

            if (blocked)
                wantsToCrouch = true;
        }

        if (_isCrouching != wantsToCrouch)
        {
            _isCrouching = wantsToCrouch;
            _characterController.height = _isCrouching ? crouchHeight : standingHeight;
            _characterController.center = new Vector3(0, _characterController.height / 2f, 0);
        }

        float targetCameraY = _isCrouching ? _cameraCrouchingY : _cameraStandingY;
        Vector3 camPos = cameraTarget.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        cameraTarget.localPosition = camPos;
    }

    private void ApplyGravity()
    {
        if (isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
            return;
        }

        _verticalVelocity += CurrentGravity * Time.deltaTime;
        _verticalVelocity = Mathf.Max(_verticalVelocity, -20f);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && !_isCrouching)
        {
            _verticalVelocity = _playerStats.jumpForce;
            _animator.SetTrigger(JumpHash);

            if (jumpClip != null)
                audioSource.PlayOneShot(jumpClip);
        }
    }

    public void Launch(float force)
    {
        _verticalVelocity = force;
    }

    private MovementAudio GetCurrentAudioSettings()
    {
        bool isSprinting = sprintAction.action.ReadValue<float>() > 0;
        bool isSneaking = sneakAction.action.ReadValue<float>() > 0 && !isSprinting;

        if (_isCrouching) return crouchSounds;
        if (isSprinting) return sprintSounds;
        if (isSneaking) return sneakSounds;
        return walkSounds;
    }

    private void HandleFootsteps()
    {
        if (isGrounded && _characterController.velocity.magnitude > 0.1f)
        {
            MovementAudio currentSettings = GetCurrentAudioSettings();

            _footstepTimer -= Time.deltaTime;

            if (_footstepTimer <= 0)
            {
                PlayFootstepSound(currentSettings);
                _footstepTimer = currentSettings.stepInterval;
            }
        }
        else
        {
            _footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound(MovementAudio settings)
    {
        if (settings.footstepClips == null || settings.footstepClips.Length == 0) return;

        // Выбираем звук
        int index = Random.Range(0, settings.footstepClips.Length);
        AudioClip clip = settings.footstepClips[index];

        float finalVolume = settings.volume * globalVolumeModifier;

        audioSource.pitch = Random.Range(0.7f, 0.9f);
        audioSource.PlayOneShot(clip, finalVolume);
    }

    private void HandleLanding()
    {
        if (!_wasGrounded && isGrounded)
        {
            if (landClip != null)
                audioSource.PlayOneShot(landClip);

            _footstepTimer = 0.2f;
        }
        _wasGrounded = isGrounded;
    }
}