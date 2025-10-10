using Unity.Netcode;

public struct ItemStat : INetworkSerializable, System.IEquatable<ItemStat>
{
    public StatType Type;
    public float Value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Type);
        serializer.SerializeValue(ref Value);
    }

    public bool Equals(ItemStat other)
        => Type == other.Type && Value.Equals(other.Value);
}
