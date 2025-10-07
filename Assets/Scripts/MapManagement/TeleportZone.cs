using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportZone : MonoBehaviour
{
    public Vector2 target;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = target;
            var rb = other.attachedRigidbody; if (rb) rb.linearVelocity = Vector2.zero;
        }
    }
}
