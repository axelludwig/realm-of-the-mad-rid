using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnemyBlue : Enemy
{
    private float desiredRange = 6f;
    private Transform target;
    private float nextFireTime = 0f;

    [SerializeField] private GameObject projectilePrefab;
    protected override void Awake()
    {
        base.Awake();

        Stats = new EntityStats(
            health: 10,
            moveSpeed: 6
        );
    }

    private void Update()
    {
        // Seul le serveur contrôle le déplacement
        if (!IsServer) return;

        // Trouver un joueur à poursuivre si aucun
        if (target == null)
        {
            target = FindClosestPlayer();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Si trop loin : approche
        if (distanceToTarget > desiredRange)
        {
            MoveTowardTarget();
        }
        // Si trop proche : recule
        else if (distanceToTarget < desiredRange * 0.9f)
        {
            MoveAwayFromTarget();
        }
        // Sinon : reste sur place et attaque
        else
        {
            TryShoot();
        }

        // Si le joueur s'éloigne trop : reset le target
        if (distanceToTarget > desiredRange)
        {
            target = null;
        }
    }

    private void MoveTowardTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * Stats.MovementSpeed.CurrentValue * Time.deltaTime;
    }

    private void MoveAwayFromTarget()
    {
        Vector3 direction = (transform.position - target.position).normalized;
        transform.position += direction * Stats.MovementSpeed.CurrentValue * Time.deltaTime;
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + Stats.AttackSpeed.CurrentValue;

        if (projectilePrefab == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        SpawnProjectileServerRpc(transform.position, direction);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc(Vector3 position, Vector2 direction)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, position, Quaternion.identity);
        var projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Initialize(direction, this);
        projectile.NetworkObject.Spawn();
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
