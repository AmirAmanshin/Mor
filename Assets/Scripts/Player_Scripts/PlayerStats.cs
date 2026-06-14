using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Player ConStats")]
    [SerializeField] public float speed = 6.0f;
    [SerializeField] public float jumpForce = 4.5f;
    //[SerializeField] private float staminaRegeneration = 15.0f;
    //[SerializeField] private float maxStamina = 100.0f;

    [Header("Player FluidStats")]
    [SerializeField] public float health = 100.0f;
    //[SerializeField] public float currentStamina = 100.0f;
    [SerializeField] public float visibility = 0;
    [SerializeField] public float noiseLevel = 0;

    [Header("World to Player Stats")]
    [SerializeField] public float illuminationLevel = 0f;


    [Header("References")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private EnemyUpgrader enemyUpgrader;


    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player took damage! Left HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead.");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        EnemyUpgrader.Instance.ResetUpgrades();
    }
}
