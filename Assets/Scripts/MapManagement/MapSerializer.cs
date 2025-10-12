using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MapSerializer
{
    static string Folder => Path.Combine(Application.persistentDataPath, "Worlds");

    public static void SaveRegion(Vector2Int region, RegionData data)
    {
        Directory.CreateDirectory(Folder);
        var path = Path.Combine(Folder, $"region_{region.x}_{region.y}.json");
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public static RegionData LoadRegion(Vector2Int region)
    {
        var path = Path.Combine(Folder, $"region_{region.x}_{region.y}.json");
        return File.Exists(path)
            ? JsonUtility.FromJson<RegionData>(File.ReadAllText(path))
            : null;
    }

    public static Dictionary<Vector2Int, RegionData> LoadAll()
    {
        var dict = new Dictionary<Vector2Int, RegionData>();
        if (!Directory.Exists(Folder)) return dict;

        foreach (var file in Directory.GetFiles(Folder, "region_*.json"))
        {
            var parts = Path.GetFileNameWithoutExtension(file).Split('_');
            if (parts.Length == 3 &&
                int.TryParse(parts[1], out int x) &&
                int.TryParse(parts[2], out int y))
            {
                var data = JsonUtility.FromJson<RegionData>(File.ReadAllText(file));
                dict[new Vector2Int(x, y)] = data;
            }
        }
        return dict;
    }
}
