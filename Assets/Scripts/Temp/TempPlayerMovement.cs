using UnityEngine;
using UnityEngine.InputSystem; // N�cessaire pour le nouveau syst�me

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
        // Appel� automatiquement si tu as une action "Move" li�e � ce script
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * status.currentSpeed;
    }
}
