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
}