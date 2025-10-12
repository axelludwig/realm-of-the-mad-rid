using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Map/Tile Registry", fileName = "TileRegistry")]
public class TileRegistry : ScriptableObject
{
    [Serializable]
    public struct Entry { public string id; public TileBase tile; }

    public Entry[] entries;
    private Dictionary<string, TileBase> _dict;

    void OnEnable() => Build();

    public void Build()
    {
        _dict = new Dictionary<string, TileBase>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
            if (!string.IsNullOrEmpty(e.id) && e.tile != null)
                _dict[e.id] = e.tile;
    }

    public TileBase Get(string id) =>
        (!string.IsNullOrEmpty(id) && _dict != null && _dict.TryGetValue(id, out var t)) ? t : null;
}
