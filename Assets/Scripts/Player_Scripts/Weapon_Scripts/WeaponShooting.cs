using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [SerializeField] private float damage = 33.34f;
    [SerializeField] private float maxRange = 150f;

    public Vector3 Shoot(Transform muzzle)
    {
        Vector3 targetPoint = muzzle.position + muzzle.forward * maxRange;

        if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hit, maxRange))
        {
            targetPoint = hit.point;
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }

        return targetPoint;
    }
}