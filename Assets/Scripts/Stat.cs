using System;
using Unity.Netcode;
using UnityEngine;

public class Stat
{
    private float baseValue;
    private float bonusValue;
    private float bonusMultiplier;
    private float currentValue;

    public float BaseValue => baseValue;
    public float BonusValue => bonusValue;
    public float BonusMultiplier => bonusMultiplier;

    Action OnCurrentValueChanged;

    public float MaxValue => (baseValue + bonusValue) * bonusMultiplier;
    public float CurrentValue
    {
        get => currentValue;
        private set => currentValue = Mathf.Clamp(value, 0, MaxValue);
    }

    public Stat(float baseValue, ref NetworkVariable<float> networkVariable)
    {
        this.baseValue = baseValue;
        bonusValue = 0;
        bonusMultiplier = 1f;
        CurrentValue = MaxValue;
        var v_networkVariable = networkVariable;

        OnCurrentValueChanged = () => {
            v_networkVariable.Value = CurrentValue;
        };
    }

    public void Decrease(float value) => CurrentValue -= value;
    public void Increase(float value) => CurrentValue += value;

    public void AddBonus(float value)
    {
        bonusValue += value;
        CurrentValue = Mathf.Min(CurrentValue + value, MaxValue);
        OnCurrentValueChanged();
    }

    public void RemoveBonus(float value)
    {
        bonusValue -= value;
        CurrentValue = Mathf.Clamp(CurrentValue - value, 0, MaxValue);
    }

    public void AddMultiplier(float value)
    {
        bonusMultiplier += value;
        CurrentValue = Mathf.Clamp(CurrentValue * (1 + value), 0, MaxValue);
    }

    public void RemoveMultiplier(float value)
    {
        bonusMultiplier -= value;
        CurrentValue = Mathf.Clamp(CurrentValue * (1 - value), 0, MaxValue);
    }
}
