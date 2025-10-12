using Unity.Collections;
using Unity.Netcode;

public struct NetItem : INetworkSerializable, System.IEquatable<NetItem>
{
    public FixedString64Bytes Name;
    public int UniqueId;
    public FixedString64Bytes GlobalId;
    public FixedList128Bytes<ItemStat> Stats;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref UniqueId);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref GlobalId);

        // 👇 Sérialisation manuelle de la FixedList
        int count = Stats.Length;
        serializer.SerializeValue(ref count);

        if (serializer.IsReader)
        {
            Stats = new Unity.Collections.FixedList128Bytes<ItemStat>();
            for (int i = 0; i < count; i++)
            {
                ItemStat stat = default;
                stat.NetworkSerialize(serializer); // on délègue au struct interne
                Stats.Add(stat);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var stat = Stats[i];
                stat.NetworkSerialize(serializer);
            }
        }
    }


    public bool Equals(NetItem other)
    {
        if (UniqueId != other.UniqueId) return false;
        if (!Name.Equals(other.Name)) return false;
        if (Stats.Length != other.Stats.Length) return false;
        for (int i = 0; i < Stats.Length; i++)
            if (!Stats[i].Equals(other.Stats[i])) return false;
        return true;
    }
}