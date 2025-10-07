using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public enum DamageType
{
    StrengthBased,
    IntelligenceBased
}

public class Entity : NetworkBehaviour
{
    /// <summary>
    /// Hp de pv de points de vie
    /// </summary>
    public Stat Health { get; private set; }

    /// <summary>
    /// R�duit les d�gats subis (100 d'armure sur 100 hp max permettent de r�duire les d�gats de 50%, enfin �a c'est mon id�e mais c pas encore impl�ment� mais �a arrive fort en gros)
    /// </summary>
    public Stat Armour { get; private set; }

    /// <summary>
    /// Vitesse de move speed, 1 par d�faut
    /// </summary>
    public Stat MovementSpeed { get; private set; }

    /// <summary>
    /// Vitesse d�attaque speed. Influence la vitesse d'attaque avec l'arme �quip�e (clic gauche). 1 par d�faut.
    /// </summary>
    public Stat AttackSpeed { get; private set; }

    /// <summary>
    /// 1 point de force = 1% de d�gats bonus avec l'arme �quip�e
    /// </summary>
    public Stat Strength { get; private set; }

    /// <summary>
    /// 1 point d'intelligence = 1% de d�gats bonus avec les sorts
    /// </summary>
    public Stat Intelligence { get; private set; }

    /// <summary>
    /// CDR de r�duction des d�lais, r�duit le CD des sorts. (exprim�e en %, 0 par d�faut).
    /// </summary>
    public Stat CooldownReduction { get; private set; }

    FloatingHealthBar healthBar;

    public NetworkVariable<float> NetworkedHealth = new NetworkVariable<float>(
        100f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );



    protected virtual void Awake()
    {
        Health = new Stat(100);
        Armour = new Stat(0);
        MovementSpeed = new Stat(1);
        AttackSpeed = new Stat(1);
        Strength = new Stat(0);
        Intelligence = new Stat(0);
        CooldownReduction = new Stat(0);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        healthBar = GetComponentInChildren<FloatingHealthBar>();

        NetworkedHealth.OnValueChanged += (oldValue, newValue) =>
        {
            if (healthBar != null)
                healthBar.UpdateHealthBar(newValue / Health.MaxValue);
        };

        healthBar?.UpdateHealthBar(NetworkedHealth.Value / Health.MaxValue);
    }

    [ServerRpc]
    public void TakeDamageServerRpc(float rawDamage, ulong dealerId)
    {
        float armorDamageMultiplier = 1f / (1f + Armour.CurrentValue / Health.CurrentValue);
        float valueAfterArmourReduction = rawDamage * armorDamageMultiplier;

        Health.Decrease(valueAfterArmourReduction);
        NetworkedHealth.Value = Health.CurrentValue;
        if (Health.CurrentValue <= 0) Die();
    }

    public void DealDamage(float rawDamage, Entity target, DamageType damageType = DamageType.StrengthBased)
    {
        if (!IsOwner) return;

        float damageMultiplier = 1 + (damageType == DamageType.StrengthBased ? Strength.CurrentValue : Intelligence.CurrentValue) / 100;
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
