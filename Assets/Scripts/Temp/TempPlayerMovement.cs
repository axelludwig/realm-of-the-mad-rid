using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour le nouveau système

[RequireComponent(typeof(Rigidbody2D))]
public class TempPlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private StatusEffectController status;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        status = GetComponent<StatusEffectController>();
    }

    void OnMove(InputValue value)
    {
        // Appelé automatiquement si tu as une action "Move" liée à ce script
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * status.currentSpeed;
    }
}
