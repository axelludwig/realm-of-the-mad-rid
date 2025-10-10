using System.Collections.Generic;
using System;

[Serializable]
public class JsonMap
{
    public int seed;
    public int tileSize;
    public List<JsonBiomeRegion> biomeRegions;
    public List<JsonPlacedObject> objects;
    public List<JsonEffectZone> effectZones;
    public List<JsonTeleport> teleports;
}
[Serializable] public class JsonBiomeRegion { public string biomeId; public float[] center; public float radius; }
[Serializable] public class JsonPlacedObject { public string id; public int[] pos; public int sizeIndex; }
[Serializable] public class JsonTeleport { public int[] from; public int[] to; }
[Serializable] public class JsonEffectSpec { public string type; public float magnitude; public float dps; }
[Serializable] public class JsonEffectZone { public List<JsonEffectSpec> effects; public List<float[]> points; }

