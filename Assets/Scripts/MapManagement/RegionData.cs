using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class RegionData
{
    public Int2 region;
    public TileData[] tiles;
    public ObjectData[] objects;
}

[Serializable]
public struct TileData : INetworkSerializable
{
    public int x, y;
    public string tileId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
        serializer.SerializeValue(ref tileId);
    }
}

[Serializable]
public struct ObjectData : INetworkSerializable
{
    public string objectId;
    public Vector2 pos;
    public int sizeIndex;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref objectId);
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref sizeIndex);
    }
}
