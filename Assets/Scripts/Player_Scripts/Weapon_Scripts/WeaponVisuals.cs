using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private CinemachineCamera firstPersonCamera;

    [Header("Hip Position & Rotation")]
    [SerializeField] private Vector3 hipPosition = new Vector3(-0.742f, -0.529f, 0.039f);
    [SerializeField] private Vector3 hipRotation = Vector3.zero;

    [Header("Aim Position & Rotation")]
    [SerializeField] private Vector3 aimPosition = new Vector3(-0.21f, -0.73f, 0.23f);
    [SerializeField] private Vector3 aimRotation = Vector3.zero;

    [Header("Animation Settings")]
    [SerializeField] private float aimSpeed = 10f;
    [SerializeField] private float fovSpeed = 10f;
    [SerializeField] private float normalFOV = 90f;
    [SerializeField] private float aimFOV = 70f;

    [Header("Effects")]
    [SerializeField] private LineRenderer lineRenderer;

    private MeshRenderer[] _weaponMeshes;
    private WaitForSeconds _tracerDuration = new WaitForSeconds(0.05f);

    private void Start()
    {
        _weaponMeshes = weaponRoot != null
            ? weaponRoot.GetComponentsInChildren<MeshRenderer>()
            : GetComponentsInChildren<MeshRenderer>();

        if (weaponRoot != null)
        {
            weaponRoot.localPosition = hipPosition;
            weaponRoot.localRotation = Quaternion.Euler(hipRotation);
        }
    }

    public void UpdateAimingVisuals(bool isAiming)
    {
        Vector3 targetPos = isAiming ? aimPosition : hipPosition;
        Vector3 targetRot = isAiming ? aimRotation : hipRotation;
        float targetFOV = isAiming ? aimFOV : normalFOV;

        weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, targetPos, Time.deltaTime * aimSpeed);

        Quaternion targetQuaternion = Quaternion.Euler(targetRot);
        weaponRoot.localRotation = Quaternion.Slerp(weaponRoot.localRotation, targetQuaternion, Time.deltaTime * aimSpeed);

        if (firstPersonCamera != null)
        {
            var lens = firstPersonCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, targetFOV, Time.deltaTime * fovSpeed);
            firstPersonCamera.Lens = lens;
        }
    }

    public void SetWeaponVisibility(bool isVisible)
    {
        if (_weaponMeshes == null) return;
        foreach (var mesh in _weaponMeshes)
        {
            if (mesh != null)
                mesh.shadowCastingMode = isVisible ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
        }
    }

    public void ShowTracer(Vector3 start, Vector3 end)
    {
        if (lineRenderer != null) StartCoroutine(RenderTracer(start, end));
    }

    private IEnumerator RenderTracer(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        yield return _tracerDuration;
        lineRenderer.enabled = false;
    }
}