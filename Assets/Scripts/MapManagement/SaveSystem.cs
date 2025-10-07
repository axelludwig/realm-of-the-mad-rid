using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string PathFile => Path.Combine(Application.persistentDataPath, "save.json");
    public static void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathFile, json);
    }
    public static SaveData Load()
    {
        if (!File.Exists(PathFile)) return new SaveData();
        return JsonUtility.FromJson<SaveData>(File.ReadAllText(PathFile));
    }
}
