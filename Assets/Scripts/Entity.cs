using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    FloatingHealthBar healthBar;
    public Experience Experience;
    public EntityStats Stats;

    public NetworkVariable<float> NetworkedHealth;

    public NetworkVariable<int> NetworkXP;

    public NetworkVariable<int> NetworkLevel;

    protected virtual void Awake()
    {
        Experience = new Experience();
        Stats = new EntityStats();

        NetworkedHealth = new NetworkVariable<float>(
            Stats.Health.CurrentValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        NetworkXP = new NetworkVariable<int>(
            Experience.ExperiencePoints,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
        NetworkLevel = new NetworkVariable<int>(
            Experience.Level,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        if (IsServer)
        {
            NetworkXP.Value = Experience.ExperiencePoints;
            NetworkLevel.Value = Experience.Level;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        healthBar = GetComponentInChildren<FloatingHealthBar>();

        NetworkedHealth.OnValueChanged += (oldValue, newValue) =>
        {
            if (healthBar != null)
                healthBar.UpdateHealthBar(newValue / Stats.Health.MaxValue);
        };

        NetworkXP.OnValueChanged += (oldValue, newValue) =>
        {
            if (IsOwner)
                Experience.SetXp(newValue);
        };

        NetworkLevel.OnValueChanged += (oldValue, newValue) =>
        {
            if (IsOwner)
                Experience.SetLevel(newValue);
        };

        healthBar?.UpdateHealthBar(NetworkedHealth.Value / Stats.Health.MaxValue);
    }

    [ServerRpc]
    public void TakeDamageServerRpc(float rawDamage, ulong dealerId)
    {
        float armorDamageMultiplier = 1f / (1f + Stats.Armour.CurrentValue / Stats.Health.CurrentValue);
        float valueAfterArmourReduction = rawDamage * armorDamageMultiplier;

        Stats.Health.Decrease(valueAfterArmourReduction);
        NetworkedHealth.Value = Stats.Health.CurrentValue;
        if (Stats.Health.CurrentValue <= 0) Die(dealerId);
    }

    public void DealDamage(float rawDamage, Entity target, DamageType damageType = DamageType.StrengthBased)
    {
        if (!IsOwner) return;

        float damageMultiplier = 1 + (damageType == DamageType.StrengthBased ? Stats.Strength.CurrentValue : Stats.Intelligence.CurrentValue) / 100;
        float valueAfterStatApplied = rawDamage * damageMultiplier;

        target.TakeDamageServerRpc(valueAfterStatApplied, OwnerClientId);
    }

    public void Die(ulong killerId)
    {
        if (!IsServer) return;

        var killer = GameManager.Instance.GetPlayerByClientId(killerId);
        if (killer != null)
        {
            var playerEntity = killer.GetComponent<Entity>();
            if (playerEntity)
            {
                int xpToGive = Experience.GetXPGivenOnDeath();
                float killerAuraRadius = playerEntity.Stats.AuraRadius.CurrentValue;
                Collider2D[] hits = Physics2D.OverlapCircleAll(playerEntity.transform.position, killerAuraRadius);

                foreach (var hit in hits)
                {
                    var entity = hit.GetComponent<Entity>();
                    if (entity != null && entity.CompareTag("Player"))
                    {
                        entity.GainXP(xpToGive);
                        Debug.Log($"- {entity.gameObject.name} gagne {xpToGive} XP (ennemi tué par {killer.name})");
                    }
                }
            }
        }

        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Stats == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Stats.AuraRadius.CurrentValue);
    }

    public void GainXP(int xp)
    {
        if (!IsServer) return;

        Experience.GainXP(xp);

        NetworkXP.Value = Experience.ExperiencePoints;
        NetworkLevel.Value = Experience.Level;

        Debug.Log($"{gameObject.name} gagne {xp} XP (Total : {Experience.ExperiencePoints}, Niveau : {Experience.Level})");
    }
}
