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
    /// Réduit les dégats subis (100 d'armure sur 100 hp max permettent de réduire les dégats de 50%, enfin ça c'est mon idée mais c pas encore implémenté mais ça arrive fort en gros)
    /// </summary>
    public Stat Armour { get; private set; }

    /// <summary>
    /// Vitesse de move speed, 5 par défaut
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

    /// <summary>
    /// Rayon de l'aura du joueur. Détermine à quelle distance l'xp est partagée, et sûrement d'autres trucs qu'on implémentera un jour (50 par défaut).
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
