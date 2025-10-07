using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRuntime : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Grid grid;
    public Tilemap groundTilemap;
    public TileBase fallbackGroundTile;

    [Header("Données")]
    public BiomeDefinition[] biomes;
    public MapObjectDefinition[] objectDefs;
    public int regionSize = 1;          // nombre de tiles par région (8x8)
    public int viewRadiusRegions = 1;   // rayon de régions à générer
    public float tileWorldSize = 1f;    // échelle d'une tuile dans le monde (1 = parfait pour PPU=8)

    private JsonMap _json;
    private Dictionary<string, BiomeDefinition> _biomeById;
    private Dictionary<string, MapObjectDefinition> _objById;
    private HashSet<Vector2Int> _loadedRegions = new();

    void Awake()
    {
        _json = MapProvider.LoadFromStreamingAssets();
        _biomeById = new(); foreach (var b in biomes) _biomeById[b.biomeId] = b;
        _objById = new(); foreach (var d in objectDefs) _objById[d.objectId] = d;
        GenerateStaticFromJson();
    }

    void Update()
    {
        var region = WorldToRegion(player.position);
        for (int ry = -viewRadiusRegions; ry <= viewRadiusRegions; ry++)
        {
            for (int rx = -viewRadiusRegions; rx <= viewRadiusRegions; rx++)
            {
                var r = new Vector2Int(region.x + rx, region.y + ry);
                if (_loadedRegions.Contains(r)) continue;
                GenerateRegion(r);
                _loadedRegions.Add(r);
            }
        }
    }

    Vector2Int WorldToRegion(Vector3 world)
    {
        int rx = Mathf.FloorToInt(world.x / (regionSize * tileWorldSize));
        int ry = Mathf.FloorToInt(world.y / (regionSize * tileWorldSize));
        return new Vector2Int(rx, ry);
    }

    void GenerateRegion(Vector2Int r)
    {
        var tile = PickGroundTileForRegion(r);
        var originX = r.x * regionSize;
        var originY = r.y * regionSize;

        for (int y = 0; y < regionSize; y++)
        {
            for (int x = 0; x < regionSize; x++)
            {
                var cell = new Vector3Int(originX + x, originY + y, 0);
                groundTilemap.SetTile(cell, tile);
            }
        }

        ScatterObjectsProc(r);
    }

    TileBase PickGroundTileForRegion(Vector2Int r)
    {
        BiomeDefinition best = null;
        float bestDist = float.MaxValue;
        var regionCenter = new Vector2((r.x + 0.5f) * regionSize, (r.y + 0.5f) * regionSize);

        foreach (var br in _json.biomeRegions)
        {
            var c = new Vector2(br.center[0], br.center[1]);
            var d = Vector2.Distance(regionCenter, c) - br.radius;
            if (d < bestDist)
            {
                bestDist = d;
                best = _biomeById.GetValueOrDefault(br.biomeId, null);
            }
        }
        return best != null ? best.defaultGround : fallbackGroundTile;
    }

    void ScatterObjectsProc(Vector2Int r)
    {
        var tile = PickGroundTileForRegion(r);
        BiomeDefinition biome = null;
        foreach (var b in biomes)
            if (b.defaultGround == tile) { biome = b; break; }
        if (biome == null || biome.spawnableObjects == null) return;

        var rng = new System.Random((_json.seed + r.x * 73856093 ^ r.y * 19349663));
        int attempts = Mathf.RoundToInt(regionSize * regionSize * biome.treeDensity);

        for (int i = 0; i < attempts; i++)
        {
            var lx = rng.Next(0, regionSize);
            var ly = rng.Next(0, regionSize);
            var world = new Vector3((lx + r.x * regionSize) * tileWorldSize,
                                    (ly + r.y * regionSize) * tileWorldSize, 0);
            var def = biome.spawnableObjects[rng.Next(0, biome.spawnableObjects.Length)];
            SpawnObject(def, world, rng.Next(0, def.sizeVariants.Length));
        }
    }

    void GenerateStaticFromJson()
    {
        if (_json.objects != null)
            foreach (var o in _json.objects)
                if (_objById.TryGetValue(o.id, out var def))
                    SpawnObject(def, new Vector3(o.pos[0], o.pos[1], 0), o.sizeIndex);

        if (_json.effectZones != null)
            foreach (var z in _json.effectZones)
                CreateEffectZone(z);

        if (_json.teleports != null)
            foreach (var t in _json.teleports)
                CreateTeleport(new Vector2(t.from[0], t.from[1]), new Vector2(t.to[0], t.to[1]));
    }

    void SpawnObject(MapObjectDefinition def, Vector3 pos, int sizeIndex)
    {
        var prefab = def.sizeVariants[Mathf.Clamp(sizeIndex, 0, def.sizeVariants.Length - 1)];
        var go = Instantiate(prefab, pos, Quaternion.identity, transform);
        var rb = go.GetComponent<Rigidbody2D>() ?? go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        var bc = go.GetComponent<BoxCollider2D>() ?? go.AddComponent<BoxCollider2D>();
        bc.size = def.colliderSize;
    }

    void CreateEffectZone(JsonEffectZone z)
    {
        var go = new GameObject("EffectZone");
        go.transform.SetParent(transform);
        var poly = go.AddComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        var pts = new Vector2[z.points.Count];
        for (int i = 0; i < pts.Length; i++) pts[i] = new Vector2(z.points[i][0], z.points[i][1]);
        poly.SetPath(0, pts);
        var ez = go.AddComponent<EffectZone>();
        foreach (var e in z.effects) ez.effects.Add(EffectSpec.FromJson(e));
    }

    void CreateTeleport(Vector2 from, Vector2 to)
    {
        var go = new GameObject("TeleportZone");
        go.transform.SetParent(transform);
        var bc = go.AddComponent<BoxCollider2D>();
        bc.isTrigger = true; bc.size = Vector2.one;
        go.transform.position = from;
        var tp = go.AddComponent<TeleportZone>();
        tp.target = to;
    }
}
