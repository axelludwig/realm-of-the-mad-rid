using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private float v_Speed = 10f;
    private Vector2 v_Direction;

    /// <summary>
    /// Initialise le projectile avec une direction donn�e.
    /// </summary>
    public void Initialize(Vector2 p_Direction)
    {
        v_Direction = p_Direction.normalized;
    }

    private void Update()
    {
        // Se d�place seulement c�t� serveur (source d'autorit�)
        if (!IsServer) return;

        transform.Translate(v_Direction * v_Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ONTRIGGER");
        if (!IsServer) return;
        // Exemple : d�truire apr�s impact
        DestroyProjectileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyProjectileServerRpc()
    {
        NetworkObject.Despawn();
    }
}
