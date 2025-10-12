using Unity.Netcode;
using UnityEngine;

public struct RegionMessage : INetworkSerializable
{
    public Vector2Int region;
    public TileData[] tiles;
    public ObjectData[] objects;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // --- Région ---
        serializer.SerializeValue(ref region);

        // --- Tiles ---
        int tileCount = tiles?.Length ?? 0;
        serializer.SerializeValue(ref tileCount);

        if (serializer.IsReader)
            tiles = new TileData[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            if (serializer.IsReader)
            {
                TileData td = default;
                td.NetworkSerialize(serializer);
                tiles[i] = td;
            }
            else
            {
                var td = tiles[i];
                td.NetworkSerialize(serializer);
            }
        }

        // --- Objects ---
        int objCount = objects?.Length ?? 0;
        serializer.SerializeValue(ref objCount);

        if (serializer.IsReader)
            objects = new ObjectData[objCount];

        for (int i = 0; i < objCount; i++)
        {
            if (serializer.IsReader)
            {
                ObjectData od = default;
                od.NetworkSerialize(serializer);
                objects[i] = od;
            }
            else
            {
                var od = objects[i];
                od.NetworkSerialize(serializer);
            }
        }
    }

    public static RegionMessage FromReader(FastBufferReader reader)
    {
        reader.ReadValueSafe(out RegionMessage msg);
        return msg;
    }
}
