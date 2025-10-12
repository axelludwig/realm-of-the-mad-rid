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
        float health = 100,
        float armour = 0,
        float moveSpeed = 5,
        float attackSpeed = 1,
        float strength = 0,
        float intelligence = 0,
        float cooldownReduction = 0,
        float auraRadius = 50)
    {
        Health = new Stat(health);
        Armour = new Stat(armour);
        MovementSpeed = new Stat(moveSpeed);
        AttackSpeed = new Stat(attackSpeed);
        Strength = new Stat(strength);
        Intelligence = new Stat(intelligence);
        CooldownReduction = new Stat(cooldownReduction);
        AuraRadius = new Stat(auraRadius);
    }

}
