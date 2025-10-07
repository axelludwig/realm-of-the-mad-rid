using System;

public class EffectSpec
{
    public EffectType type;
    public float magnitude;
    public float dps;

    public static EffectSpec FromJson(JsonEffectSpec j)
    {
        var e = new EffectSpec();
        e.type = Enum.TryParse<EffectType>(j.type, true, out var t) ? t : EffectType.Slow;
        e.magnitude = j.magnitude;
        e.dps = j.dps;
        return e;
    }
}

