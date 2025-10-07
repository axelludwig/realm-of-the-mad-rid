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
    /// Réduit les dégats subis (100 d'armure sur 100 hp max permettent de réduire les dégats de 50%, enfin ça c'est mon idée mais c pas encore implémenté mais ça arrive fort en gros)
    /// </summary>
    public Stat Armour { get; private set; }

    /// <summary>
    /// Vitesse de move speed, 1 par défaut
    /// </summary>
    public Stat MovementSpeed { get; private set; }

    /// <summary>
    /// Vitesse d’attaque speed. Influence la vitesse d'attaque avec l'arme équipée (clic gauche). 1 par défaut.
    /// </summary>
    public Stat AttackSpeed { get; private set; }

    /// <summary>
    /// 1 point de force = 1% de dégats bonus avec l'arme équipée
    /// </summary>
    public Stat Strength { get; private set; }

    /// <summary>
    /// 1 point d'intelligence = 1% de dégats bonus avec les sorts
    /// </summary>
    public Stat Intelligence { get; private set; }

    /// <summary>
    /// CDR de réduction des délais, réduit le CD des sorts. (exprimée en %, 0 par défaut).
    /// </summary>
    public Stat CooldownReduction { get; private set; }

    FloatingHealthBar healthBar;

    protected virtual void Awake()
    {
        Debug.Log("Awaking Umtity");
        Health = new Stat(100);
        Armour = new Stat(0);
        MovementSpeed = new Stat(1);
        AttackSpeed = new Stat(1);
        Strength = new Stat(0);
        Intelligence = new Stat(0);
        CooldownReduction = new Stat(0);
    }

    public void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(Health.CurrentValue / Health.MaxValue);
    }

    public void TakeDamage(float rawDamage, Entity dealer)
    {
        float armorDamageMultiplier = 1f / (1f + Armour.CurrentValue / Health.CurrentValue);
        float valueAfterArmourReduction = rawDamage * armorDamageMultiplier;
        Health.Decrease(valueAfterArmourReduction);
        healthBar.UpdateHealthBar(Health.CurrentValue / Health.MaxValue);
        if (Health.CurrentValue <= 0) Die();
    }

    public void DealDamage(float rawDamage, Entity target, DamageType damageType = DamageType.StrengthBased)
    {
        float damageMultiplier = 1 + (damageType == DamageType.StrengthBased ? Strength.CurrentValue : Intelligence.CurrentValue) / 100;
        float valueAfterStatApplied = rawDamage * damageMultiplier;
        target.TakeDamage(valueAfterStatApplied, this);
    }

    public void Die()
    {
        Debug.Log("I dieded");
    }
}
