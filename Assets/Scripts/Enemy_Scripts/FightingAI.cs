using UnityEngine;
using UnityEngine.AI;

public class FightingAI : MonoBehaviour, IDamageable
{
    public float currentHealth = 100.0f;
    [SerializeField] private PlayerUI _playerUI;
    public bool isAlive = true;

    [Header("Attack Settings")]
    public float damageAmount = 25f;
    public float attackCooldown = 1.0f;
    private float nextAttackTime = 0.1f;
    public float speed = 3.0f;

    private void Start()
    {
        GameObject playerContainer = GameObject.FindWithTag("Player");
        if (playerContainer != null)
        {
            _playerUI = playerContainer.GetComponent<PlayerUI>();
        }

        if (EnemyUpgrader.Instance != null)
        {
            // Обращаемся к свойствам (с большой буквы)
            attackCooldown = EnemyUpgrader.Instance.CurrentEnemyAttackCooldown;
            speed = EnemyUpgrader.Instance.CurrentEnemySpeed;
        }

        // Передаем скорость в NavMeshAgent прямо здесь и сейчас
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
            Debug.Log("Скорость НавМеш Агента установлена на: " + agent.speed);
        }
    }

    private void OnTriggerStay(Collider other)
    {
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
            isAlive = false;
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
}