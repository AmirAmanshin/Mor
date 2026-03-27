using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField] public float health = 100.0f;
    [SerializeField] public float visibility = 100.0f;
    [SerializeField] public float speed = 5.0f;
    [SerializeField] public float jumpForce = 5.0f;
    public Slider healthSlider;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Игрок получил урон! Осталось HP: " + health);

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player is dead.");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Загружаем её заново
        SceneManager.LoadScene(currentSceneIndex);
    }

    //[Header("Crouch Settings")]
    //[SerializeField] public float crouchHeight = 1f;  // Высота приседа (половина от стандартных 2м)
    //[SerializeField] public float standHeight = 2f;   // Обычная высота
    //[SerializeField] public float crouchSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = health;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
