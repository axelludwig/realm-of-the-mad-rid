using System.Collections.Generic;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    public float baseSpeed = 4f;
    public float currentSpeed => baseSpeed * _speedMultiplier;
    float _speedMultiplier = 1f;
    float _dotBuffer = 0f;

    public void ApplyStack(List<EffectSpec> specs, float dt)
    {
        float slowMul = 1f;
        float dps = 0f;
        foreach (var e in specs)
        {
            switch (e.type)
            {
                case EffectType.Slow: slowMul *= Mathf.Clamp(e.magnitude, 0.1f, 1f); break;
                case EffectType.Fire:
                case EffectType.Poison: dps += e.dps; break;
            }
        }
        _speedMultiplier = slowMul;
        if (dps > 0f)
        {
            _dotBuffer += dps * dt;
            if (_dotBuffer >= 1f) { DealDamage(Mathf.FloorToInt(_dotBuffer)); _dotBuffer -= Mathf.Floor(_dotBuffer); }
        }
    }

    void LateUpdate() { _speedMultiplier = 1f; }
    void DealDamage(int amount) { /* TODO: intégrer au système de vie */ }
}

