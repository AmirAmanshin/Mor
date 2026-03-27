using UnityEngine;

public class FightingAI : MonoBehaviour, IDamageable
{
    [SerializeField] public float currentHealth;
    [SerializeField] public float speed = 3.0f;
    public bool isAlive = true;

    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public float attackCooldown = 1.0f;
    private float nextAttackTime;

    private void OnTriggerStay(Collider other)
    {
        // Проверяем время и то, что мы коснулись именно игрока
        if (Time.time >= nextAttackTime && other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
            Die();
        }
        else
        {
            Debug.Log(gameObject.name + " hit! It's health now: " + currentHealth);
        }
        
    }

    void Die()
    {
        Destroy(transform.root.gameObject);
    }

    void Start()
    {
        currentHealth = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
