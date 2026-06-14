using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(WeaponShooting))]
[RequireComponent(typeof(WeaponReloading))]
[RequireComponent(typeof(WeaponVisuals))]
[RequireComponent(typeof(WeaponCrosshair))]
[RequireComponent(typeof(WeaponAudio))]
public class WeaponHub : MonoBehaviour
{
    [Header("Shared References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private PlayerStealthState _stealthState;

    [Header("Inputs")]
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference aimAction;
    [SerializeField] private InputActionReference reloadAction;

    public bool IsAiming { get; private set; }

    private WeaponShooting _shooting;
    private WeaponReloading _reloading;
    private WeaponVisuals _visuals;
    private WeaponCrosshair _crosshair;
    private WeaponAudio _audio;

    private void Awake()
    {
        _shooting = GetComponent<WeaponShooting>();
        _reloading = GetComponent<WeaponReloading>();
        _visuals = GetComponent<WeaponVisuals>();
        _crosshair = GetComponent<WeaponCrosshair>();
        _audio = GetComponent<WeaponAudio>();
        _stealthState = GetComponentInParent<PlayerStealthState>();
    }

    private void OnEnable()
    {
        attackAction.action.performed += OnFire;
        aimAction.action.performed += OnAim;
        aimAction.action.canceled += OnAim;
        reloadAction.action.performed += OnReload;

        attackAction.action.Enable();
        aimAction.action.Enable();
        reloadAction.action.Enable();

        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnFire;
        aimAction.action.performed -= OnAim;
        aimAction.action.canceled -= OnAim;
        reloadAction.action.performed -= OnReload;

        attackAction.action.Disable();
        aimAction.action.Disable();
        reloadAction.action.Disable();

        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        IsAiming = context.performed;
    }

    private void Update()
    {
        if (_reloading.IsReloading) return;

        _visuals.UpdateAimingVisuals(IsAiming);
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        _crosshair.UpdateCrosshair(muzzle, brain.OutputCamera);
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (_reloading.IsReloading) return;

        if (_reloading.HasAmmo())
        {
            _reloading.ConsumeAmmo();
            _audio.PlayShoot();

            if (_stealthState != null) _stealthState.TriggerNoiseSpike(80f);

            Vector3 hitPoint = _shooting.Shoot(muzzle);
            _visuals.ShowTracer(muzzle.position, hitPoint);
        }
        else
        {
            _audio.PlayEmpty();

            if (_stealthState != null) _stealthState.TriggerNoiseSpike(60f);

            _reloading.StartReload(_visuals);
            _audio.PlayReload();
        }
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_reloading.IsReloading) return;

        if (_stealthState != null) _stealthState.TriggerNoiseSpike(40f);

        _reloading.StartReload(_visuals);
        _audio.PlayReload();
    }
}