using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public float health = 100.0f;
    [SerializeField] public float visibility = 100.0f;
    [SerializeField] public float speed = 5.0f;
    [SerializeField] public float jumpForce = 5.0f;

    //[Header("Crouch Settings")]
    //[SerializeField] public float crouchHeight = 1f;  // Высота приседа (половина от стандартных 2м)
    //[SerializeField] public float standHeight = 2f;   // Обычная высота
    //[SerializeField] public float crouchSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
