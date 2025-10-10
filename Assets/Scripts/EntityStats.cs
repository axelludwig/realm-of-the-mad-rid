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

    public EntityStats()
    {
        Health = new Stat(100);
        Armour = new Stat(0);
        MovementSpeed = new Stat(1);
        AttackSpeed = new Stat(1);
        Strength = new Stat(0);
        Intelligence = new Stat(0);
        CooldownReduction = new Stat(0);
    }

}
