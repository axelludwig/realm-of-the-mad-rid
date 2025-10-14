using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Enemy: Entity
{
    private Transform v_Target;

    protected override void Awake()
    {
        base.Awake();

        Stats = new EntityStats(
            this,
            health: 25,
            moveSpeed: 4
        );
    }

    private void Update()
    {
        // Seul le serveur contrôle le déplacement
        if (!IsServer) return;

        // Trouver un joueur à poursuivre si aucun
        if (v_Target == null)
        {
            v_Target = FindClosestPlayer();
            return;
        }

        // Déplacement vers le joueur
        Vector3 v_Direction = (v_Target.position - transform.position).normalized;
        transform.position += v_Direction * Stats.MovementSpeed.CurrentValue * Time.deltaTime;
    }

    /// <summary>
    /// Retourne le joueur le plus proche.
    /// </summary>
    private Transform FindClosestPlayer()
    {
        var v_Players = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None)
            .Where(obj => obj.CompareTag("Player"))
            .Select(obj => obj.transform);

        Transform v_Closest = null;
        float v_MinDist = float.MaxValue;

        foreach (var v_Player in v_Players)
        {
            float v_Dist = Vector3.Distance(transform.position, v_Player.position);
            if (v_Dist < v_MinDist)
            {
                v_MinDist = v_Dist;
                v_Closest = v_Player;
            }
        }

        return v_Closest;
    }
}
