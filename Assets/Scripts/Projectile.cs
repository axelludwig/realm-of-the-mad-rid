using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private float v_Speed = 10f;
    private float range = 10f;
    private Vector3 spawnPosition;
    private Vector2 v_Direction;

    /// <summary>
    /// Initialise le projectile avec une direction donn�e.
    /// </summary>
    public void Initialize(Vector2 p_Direction)
    {
        v_Direction = p_Direction.normalized;
        spawnPosition = transform.position;
    }

    private void Update()
    {
        // Se d�place seulement c�t� serveur (source d'autorit�)
        if (!IsServer) return;

        transform.Translate(v_Direction * v_Speed * Time.deltaTime);

        float distanceTravelled = Vector3.Distance(spawnPosition, transform.position);
        if (distanceTravelled >= range)
        {
            DestroyProjectileServerRpc();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        var entity = other.gameObject.GetComponent<Enemy>();
        if(entity != null)
        {
            entity.TakeDamageServerRpc(10, OwnerClientId);
            DestroyProjectileServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        NetworkObject.Despawn();
    }
}
