using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private int jumpForce;

    private void OnTriggerEnter(Collider playerCollider)
    {
        if (playerCollider.TryGetComponent<PlayerMovement>(out var movement))
        {
            movement.Launch(jumpForce);
        }
    }
}
