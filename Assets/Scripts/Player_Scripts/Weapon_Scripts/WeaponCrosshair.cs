using UnityEngine;

public class WeaponCrosshair : MonoBehaviour
{
    [SerializeField] private RectTransform floatingCrosshair;

    [Header("Settings")]
    [Tooltip("Layers that crosshair does not interact with")]
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private float smoothSpeed = 20f;

    // Добавлен параметр activeCamera
    public void UpdateCrosshair(Transform muzzle, Camera activeCamera)
    {
        if (activeCamera == null) return;

        Vector3 targetPoint = GetHitPoint(muzzle);
        Vector3 screenPosition = activeCamera.WorldToScreenPoint(targetPoint);

        if (screenPosition.z > 0)
        {
            floatingCrosshair.position = Vector3.Lerp(floatingCrosshair.position, screenPosition, Time.deltaTime * smoothSpeed);
            ToggleCrosshair(true);
        }
        else
        {
            ToggleCrosshair(false);
        }
    }

    public void ToggleCrosshair(bool state)
    {
        if (floatingCrosshair != null && floatingCrosshair.gameObject.activeSelf != state)
        {
            floatingCrosshair.gameObject.SetActive(state);
        }
    }

    private Vector3 GetHitPoint(Transform muzzle)
    {
        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, Mathf.Infinity, ~ignoreMask))
        {
            return hit.point;
        }
        return muzzle.position + muzzle.forward * 150f;
    }
}