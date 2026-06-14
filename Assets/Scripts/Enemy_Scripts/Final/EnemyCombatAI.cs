using UnityEngine;

public class EnemyCombatAI : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Shooting")]
    public float attackDamage = 15f;
    public float fireRate = 1.2f;
    public Transform gunBarrel;

    public LayerMask hitMask;

    private float nextFireTime = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Gustav had damage: {damage}. Left HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ♫♫♫ Hermann methods ♫♫♫

    public void FireAt(Vector3 targetPosition)
    {
        if (isDead || Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        Vector3 shootDirection = (targetPosition - gunBarrel.position).normalized;

        if (Physics.Raycast(gunBarrel.position, shootDirection, out RaycastHit hit, 50f, hitMask))
        {
            Debug.Log($"Gustav shot and hitted {hit.collider.name}");

            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy is dead.");
    }
}