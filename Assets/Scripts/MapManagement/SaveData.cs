using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RegionSave
{
    public Vector2Int region;          // coordonnée de la région
    public TileData[] tiles;           // ou ton format spécifique
    public ObjectData[] objects;
}

[System.Serializable]
public class SaveData
{
    public List<RegionSave> regions = new();
}

