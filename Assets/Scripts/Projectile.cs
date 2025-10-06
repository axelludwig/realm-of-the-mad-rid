using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private float v_Speed = 10f;
    private Vector2 v_Direction;

    /// <summary>
    /// Initialise le projectile avec une direction donnée.
    /// </summary>
    public void Initialize(Vector2 p_Direction)
    {
        v_Direction = p_Direction.normalized;
    }

    private void Update()
    {
        // Se déplace seulement côté serveur (source d'autorité)
        if (!IsServer) return;

        transform.Translate(v_Direction * v_Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ONTRIGGER");
        if (!IsServer) return;
        // Exemple : détruire après impact
        DestroyProjectileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        NetworkObject.Despawn();
    }
}
