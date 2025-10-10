using Unity.Netcode;

public class Entity : NetworkBehaviour
{
    FloatingHealthBar healthBar;
    EntityStats Stats = new EntityStats();

    public NetworkVariable<float> NetworkedHealth = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        healthBar = GetComponentInChildren<FloatingHealthBar>();

        NetworkedHealth.OnValueChanged += (oldValue, newValue) =>
        {
            if (healthBar != null)
                healthBar.UpdateHealthBar(newValue / Stats.Health.MaxValue);
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
        if (Stats.Health.CurrentValue <= 0) Die();
    }

    public void DealDamage(float rawDamage, Entity target, DamageType damageType = DamageType.StrengthBased)
    {
        if (!IsOwner) return;

        float damageMultiplier = 1 + (damageType == DamageType.StrengthBased ? Stats.Strength.CurrentValue : Stats.Intelligence.CurrentValue) / 100;
        float valueAfterStatApplied = rawDamage * damageMultiplier;

        target.TakeDamageServerRpc(valueAfterStatApplied, OwnerClientId);
    }

    public void Die()
    {
        if(IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
