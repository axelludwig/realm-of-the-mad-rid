using UnityEngine;

public enum DamageType
{
    StrengthBased,
    IntelligenceBased
}

public class EntityStats
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
    /// Vitesse de move speed, 5 par d�faut
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

    /// <summary>
    /// Rayon de l'aura du joueur. D�termine � quelle distance l'xp est partag�e, et s�rement d'autres trucs qu'on impl�mentera un jour (50 par d�faut).
    /// </summary>
    public Stat AuraRadius { get; private set; }

    public EntityStats(
        Entity entity,
        float health = 100,
        float armour = 0,
        float moveSpeed = 5,
        float attackSpeed = 1,
        float strength = 0,
        float intelligence = 0,
        float cooldownReduction = 0,
        float auraRadius = 50)
    {
        Health = new Stat(health, ref entity.NetworkedHealth);
        Armour = new Stat(armour, ref entity.NetworkedArmour);
        MovementSpeed = new Stat(moveSpeed, ref entity.NetworkedMovementSpeed);
        AttackSpeed = new Stat(attackSpeed, ref entity.NetworkedAttackSpeed);
        Strength = new Stat(strength, ref entity.NetworkedStrength);
        Intelligence = new Stat(intelligence, ref entity.NetworkedIntelligence);
        CooldownReduction = new Stat(cooldownReduction, ref entity.NetworkedCooldownReduction);
        AuraRadius = new Stat(auraRadius, ref entity.NetworkedAuraRadius);
    }

}
