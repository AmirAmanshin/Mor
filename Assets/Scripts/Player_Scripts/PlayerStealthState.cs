using UnityEngine;

public class PlayerStealthState : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;

    [Header("Input Actions")]
    [SerializeField] private UnityEngine.InputSystem.InputActionReference moveAction;
    [SerializeField] private UnityEngine.InputSystem.InputActionReference sprintAction;
    [SerializeField] private UnityEngine.InputSystem.InputActionReference crouchAction;
    [SerializeField] private UnityEngine.InputSystem.InputActionReference sneakAction;

    [Header("Smoothing")]
    [Tooltip("Noise level change speed for movement")]
    [SerializeField] private float noiseTransitionSpeed = 5f;
    [Tooltip("How fast loud noises (like gunshots) decay")]
    [SerializeField] private float noiseSpikeDecayRate = 50f;
    [Tooltip("How quickly the enemy notices movement/light exposure")]
    [SerializeField] private float visibilityIncreaseSpeed = 15f;
    [Tooltip("How long the enemy 'watches' the player after they go into the shadows")]
    [SerializeField] private float visibilityDecreaseSpeed = 3f;

    private float _baseStanceVisibility;
    private float _movementPenalty;

    private float _targetVisibility;
    private float _targetNoise;

    private float _noiseSpikeLevel;

    private void Update()
    {
        CalculateTargets();

        _targetVisibility = (_baseStanceVisibility * playerStats.illuminationLevel) + _movementPenalty;
        _targetVisibility = Mathf.Clamp(_targetVisibility, 0f, 100f);

        if (_targetVisibility > playerStats.visibility)
        {
            playerStats.visibility = Mathf.Lerp(playerStats.visibility, _targetVisibility, Time.deltaTime * visibilityIncreaseSpeed);
        }
        else
        {
            playerStats.visibility = Mathf.Lerp(playerStats.visibility, _targetVisibility, Time.deltaTime * visibilityDecreaseSpeed);
        }

        if (_noiseSpikeLevel > 0)
        {
            _noiseSpikeLevel -= Time.deltaTime * noiseSpikeDecayRate;
            _noiseSpikeLevel = Mathf.Max(_noiseSpikeLevel, 0f);
        }

        float finalTargetNoise = Mathf.Max(_targetNoise, _noiseSpikeLevel);

        playerStats.noiseLevel = Mathf.Lerp(playerStats.noiseLevel, finalTargetNoise, Time.deltaTime * noiseTransitionSpeed);
    }

    public void TriggerNoiseSpike(float intensity)
    {
        _noiseSpikeLevel = Mathf.Max(_noiseSpikeLevel, intensity);
        playerStats.noiseLevel = Mathf.Max(playerStats.noiseLevel, intensity);
    }

    private void CalculateTargets()
    {
        bool isMoving = moveAction.action.ReadValue<Vector2>().magnitude > 0.1f;
        bool isSprinting = sprintAction.action.ReadValue<float>() > 0;
        bool isCrouching = crouchAction.action.ReadValue<float>() > 0;
        bool isSneaking = sneakAction.action.ReadValue<float>() > 0;
        bool isJumping = !playerMovement.isGrounded;

        if (isSneaking && isSprinting) isSneaking = false;
        if (isSneaking && isCrouching) isSneaking = false;

        if (isJumping)
        {
            _baseStanceVisibility = 35f;
            _movementPenalty = 55f;
            _targetNoise = 90f;
            return;
        }

        if (!isMoving)
        {
            _baseStanceVisibility = isCrouching ? 20f : 35f;
            _movementPenalty = 0f;
            _targetNoise = 0f;
            return;
        }

        if (isSprinting && isCrouching)
        {
            _baseStanceVisibility = 20f;
            _movementPenalty = 50f;
            _targetNoise = 80f;
        }
        else if (isSprinting) // Спринт
        {
            _baseStanceVisibility = 35f;
            _movementPenalty = 65f; // (35 + 65 = 100 максимум)
            _targetNoise = 100f;
        }
        else if (isCrouching) // Шаг в приседе
        {
            _baseStanceVisibility = 20f;
            _movementPenalty = 30f; // (20 + 30 = 50 максимум)
            _targetNoise = 65f;
        }
        else if (isSneaking) // Скрытный шаг
        {
            _baseStanceVisibility = 35f;
            _movementPenalty = 10f; // (35 + 10 = 45 максимум)
            _targetNoise = 10f;
        }
        else // Обычный шаг
        {
            _baseStanceVisibility = 35f;
            _movementPenalty = 30f; // (35 + 30 = 65 максимум)
            _targetNoise = 50f;
        }
    }
}