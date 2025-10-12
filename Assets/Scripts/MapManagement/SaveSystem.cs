using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    static string PathFile => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("⚠️ SaveSystem.Save() appelé avec un objet null — création d’une sauvegarde vide.");
            data = new SaveData();
        }

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(PathFile, json);
        Debug.Log($"💾 Monde sauvegardé : {PathFile}");
    }

    public static SaveData Load()
    {
        // 1️⃣ Fichier manquant → retourne un nouvel objet vide
        if (!File.Exists(PathFile))
        {
            Debug.LogWarning($"⚠️ Aucun fichier de sauvegarde trouvé à {PathFile}");
            return new SaveData();
        }

        // 2️⃣ Lecture du fichier
        var json = File.ReadAllText(PathFile);
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("⚠️ Le fichier de sauvegarde est vide — création d’un nouveau SaveData()");
            return new SaveData();
        }

        // 3️⃣ Désérialisation
        var data = JsonUtility.FromJson<SaveData>(json);

        // 4️⃣ Sécurité : si la liste est null, on la crée
        if (data == null)
        {
            Debug.LogError("❌ Échec de la désérialisation du fichier JSON — format invalide ?");
            return new SaveData();
        }

        if (data.regions == null)
        {
            Debug.LogWarning("⚠️ Liste 'regions' absente du JSON — initialisation vide.");
            data.regions = new List<RegionSave>();
        }

        Debug.Log($"✅ Fichier chargé depuis {PathFile} avec {data.regions.Count} régions.");
        return data;
    }
}
