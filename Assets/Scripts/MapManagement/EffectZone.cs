using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class EffectZone : MonoBehaviour
{
    public List<EffectSpec> effects = new();
    void OnTriggerStay2D(Collider2D other)
    {
        var eff = other.GetComponent<StatusEffectController>();
        if (eff) eff.ApplyStack(effects, Time.deltaTime);
    }
}
