using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour
{
    [Serializable]
    public struct Entry { public string id; public GameObject prefab; }

    public Entry[] entries;
    private Dictionary<string, GameObject> _dict;

    void Awake() => Build();

    public void Build()
    {
        _dict = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        if (entries == null) return;
        foreach (var e in entries)
            if (!string.IsNullOrEmpty(e.id) && e.prefab != null)
                _dict[e.id] = e.prefab;
    }

    public GameObject GetPrefab(string id) =>
        (!string.IsNullOrEmpty(id) && _dict != null && _dict.TryGetValue(id, out var p)) ? p : null;
}
