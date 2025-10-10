using System.Collections.Generic;
using System;
using Unity.Netcode;

[Serializable]
public struct ItemStatNetworkData : INetworkSerializable, IEquatable<ItemStatNetworkData>
{
    public StatType v_StatType;
    public float v_Value;

    /// <summary>
    /// Sérialise une stat d'item (type + valeur).
    /// </summary>
    public void NetworkSerialize<T>(BufferSerializer<T> p_Serializer) where T : IReaderWriter
    {
        p_Serializer.SerializeValue(ref v_StatType);
        p_Serializer.SerializeValue(ref v_Value);
    }

    public bool Equals(ItemStatNetworkData p_Other) => v_StatType == p_Other.v_StatType && v_Value.Equals(p_Other.v_Value);
}

[Serializable]
public struct ItemNetworkData : INetworkSerializable
{
    public string v_ItemName;
    public List<ItemStatNetworkData> v_Stats;

    /// <summary>
    /// Sérialise le nom + la liste de stats (taille + éléments).
    /// </summary>
    public void NetworkSerialize<T>(BufferSerializer<T> p_Serializer) where T : IReaderWriter
    {
        p_Serializer.SerializeValue(ref v_ItemName);

        int v_Count = (v_Stats != null) ? v_Stats.Count : 0;
        p_Serializer.SerializeValue(ref v_Count);

        if (p_Serializer.IsReader)
        {
            if (v_Stats == null) v_Stats = new List<ItemStatNetworkData>(v_Count);
            else { v_Stats.Clear(); v_Stats.Capacity = v_Count; }

            for (int v_I = 0; v_I < v_Count; v_I++)
            {
                var v_Entry = default(ItemStatNetworkData);
                p_Serializer.SerializeValue(ref v_Entry); // <- sérialise chaque struct
                v_Stats.Add(v_Entry);
            }
        }
        else
        {
            for (int v_I = 0; v_I < v_Count; v_I++)
            {
                var v_Entry = v_Stats[v_I];
                p_Serializer.SerializeValue(ref v_Entry);
            }
        }
    }
}