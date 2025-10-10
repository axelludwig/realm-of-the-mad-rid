using System.Collections.Generic;
using UnityEngine;

public class ItemInstance
{
    public ItemData Data { get; private set; }
    public Dictionary<StatType, float> FinalStats { get; private set; }

    /// <summary>
    /// Construit une instance locale (utilisée côté serveur lors de la génération).
    /// </summary>
    public ItemInstance(ItemData p_Data)
    {
        Data = p_Data;
        FinalStats = new Dictionary<StatType, float>();

        foreach (var v_Stat in p_Data.StatBonuses)
        {
            float v_Value = v_Stat.GetRandomValue();
            FinalStats.Add(v_Stat.StatType, v_Value);
        }
    }

    /// <summary>
    /// Convertit cette instance en données réseau pour synchronisation via Netcode.
    /// </summary>
    public ItemNetworkData ToNetworkData()
    {
        var v_Data = new ItemNetworkData
        {
            v_ItemName = Data.ItemName,
            v_Stats = new List<ItemStatNetworkData>()
        };

        foreach (var kvp in FinalStats)
        {
            v_Data.v_Stats.Add(new ItemStatNetworkData
            {
                v_StatType = kvp.Key,
                v_Value = kvp.Value
            });
        }

        return v_Data;
    }

    /// <summary>
    /// Reconstruit une instance locale à partir des données réseau reçues.
    /// </summary>
    public static ItemInstance FromNetworkData(ItemNetworkData p_Data, ItemDatabase p_Database)
    {
        ItemData v_ItemData = p_Database.GetItemByName(p_Data.v_ItemName);
        if (v_ItemData == null)
        {
            Debug.LogWarning($"Impossible de trouver l'item '{p_Data.v_ItemName}' dans la base !");
            return null;
        }

        var v_Instance = new ItemInstance(v_ItemData);
        v_Instance.FinalStats.Clear();

        foreach (var v_Stat in p_Data.v_Stats)
            v_Instance.FinalStats[v_Stat.v_StatType] = v_Stat.v_Value;

        return v_Instance;
    }

    /// <summary>
    /// Retourne une chaîne lisible pour debug.
    /// </summary>
    public override string ToString()
    {
        string v_Stats = "";
        foreach (var kvp in FinalStats)
            v_Stats += $"{kvp.Key}: {kvp.Value:F1}, ";
        return $"{Data.ItemName} [{v_Stats.TrimEnd(',', ' ')}]";
    }
}