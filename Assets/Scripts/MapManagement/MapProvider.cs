using System.IO;
using UnityEngine;

public static class MapProvider
{
    public static JsonMap LoadFromStreamingAssets(string filename = "map.json")
    {
        var path = Path.Combine(Application.streamingAssetsPath, filename);
        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<JsonMap>(json);
    }
}

