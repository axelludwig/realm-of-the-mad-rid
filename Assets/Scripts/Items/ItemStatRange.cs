using UnityEngine;

[System.Serializable]
public class EquipmentStatRange
{
    public StatType StatType;
    public float MinValue;
    public float MaxValue;

    /// <summary>
    /// Renvoie une valeur aléatoire dans la plage min/max pour cet item.
    /// </summary>
    public float GetRandomValue()
    {
        return Random.Range(MinValue, MaxValue);
    }
}

public enum StatType
{
    HealthFlat,
    HealthPercent,
    ArmourFlat,
    ArmourPercent,
    MovementSpeedFlat,
    MovementSpeedPercent,
    AttackSpeedFlat,
    AttackSpeedPercent,
    StrengthFlat,
    StrengthPercent,
    IntelligenceFlat,
    IntelligencePercent,
    CooldownReductionFlat,
    CooldownReductionPercent,
    AuraRadiusFlat,
    AuraRadiusPercent
}