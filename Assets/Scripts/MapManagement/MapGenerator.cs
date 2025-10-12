using UnityEngine;
using System;

public static class MapGenerator
{
    public static RegionData GenerateRegion(Vector2Int region)
    {
        var rng = new System.Random(region.x * 73856093 ^ region.y * 19349663);
        var data = new RegionData { region = new Int2(region.x, region.y) };

        int size = 8; // par exemple
        data.tiles = new TileData[size * size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                data.tiles[y * size + x] = new TileData
                {
                    x = region.x * size + x,
                    y = region.y * size + y,
                    // ici la génération
                    tileId = "Grass"
                };
            }

        data.objects = new ObjectData[rng.Next(1, 5)];
        for (int i = 0; i < data.objects.Length; i++)
        {
            data.objects[i] = new ObjectData
            {
                objectId = "Tree",
                pos = new Vector2(region.x * size + rng.Next(size), region.y * size + rng.Next(size)),
                sizeIndex = 0
            };
        }

        return data;
    }
}
