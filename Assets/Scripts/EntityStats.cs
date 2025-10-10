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
