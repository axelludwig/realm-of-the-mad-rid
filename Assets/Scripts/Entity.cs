using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    FloatingHealthBar healthBar;
    public Experience Experience;
    public EntityStats Stats;

    #region Init network variable

    public NetworkVariable<float> NetworkedHealth = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedArmour = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedMovementSpeed = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedAttackSpeed = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedStrength = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedIntelligence = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedCooldownReduction = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );
    public NetworkVariable<float> NetworkedAuraRadius = new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    #endregion

    public NetworkVariable<int> NetworkXP;

    public NetworkVariable<int> NetworkLevel;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Experience = new Experience();

        if (IsServer)
        {
            NetworkedHealth.Value = Stats.Health.CurrentValue;
            NetworkedArmour.Value = Stats.Armour.CurrentValue;
            NetworkedMovementSpeed.Value = Stats.MovementSpeed.CurrentValue;
            NetworkedAttackSpeed.Value = Stats.AttackSpeed.CurrentValue;
            NetworkedStrength.Value = Stats.Strength.CurrentValue;
            NetworkedIntelligence.Value = Stats.Intelligence.CurrentValue;
            NetworkedCooldownReduction.Value = Stats.CooldownReduction.CurrentValue;
            NetworkedAuraRadius.Value = Stats.AuraRadius.CurrentValue;

            NetworkXP.Value = Experience.ExperiencePoints;
            NetworkLevel.Value = Experience.Level;
        }


        healthBar = GetComponentInChildren<FloatingHealthBar>();

        NetworkedHealth.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log("Inside networked health on value changed");
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

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float rawDamage, ulong dealerId)
    {
        float armorDamageMultiplier = 1f / (1f + Stats.Armour.CurrentValue / Stats.Health.CurrentValue);
        float valueAfterArmourReduction = rawDamage * armorDamageMultiplier;

        Stats.Health.Decrease(valueAfterArmourReduction);
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
