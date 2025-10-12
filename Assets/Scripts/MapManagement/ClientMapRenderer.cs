using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClientMapRenderer : MonoBehaviour
{
    public TileRegistry tileRegistry;
    public ObjectRegistry objectRegistry;
    public GameObject regionPrefab; // 🔥 Prefab contenant un Grid + TilemapRenderer

    // On garde une référence par région
    private readonly Dictionary<Vector2Int, Tilemap> _regionTilemaps = new();

    private const int RegionSize = 8; // ⚠️ adapte à ta taille réelle de région

    void Start()
    {
        StartCoroutine(WaitForNetwork());
    }

    private IEnumerator WaitForNetwork()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening);
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ReceiveRegion", OnReceiveRegion);
    }

    private void OnReceiveRegion(ulong clientId, FastBufferReader reader)
    {
        Debug.Log($"📦 Message 'ReceiveRegion' reçu du serveur !");
        var msg = RegionMessage.FromReader(reader);
        Debug.Log($"🧩 Région {msg.region} reçue avec {Utils.SafeCount(msg.tiles)} tiles");
        RenderRegion(msg);
    }


    private void RenderRegion(RegionMessage msg)
    {
        Debug.Log($"🎨 Début du rendu région {msg.region}");

        if (!_regionTilemaps.TryGetValue(msg.region, out var tilemap))
        {
            var regionObj = Instantiate(regionPrefab, transform);
            var worldPos = new Vector3(msg.region.x * RegionSize, msg.region.y * RegionSize, 0);
            regionObj.transform.position = worldPos;

            tilemap = regionObj.GetComponentInChildren<Tilemap>();
            _regionTilemaps[msg.region] = tilemap;

            Debug.Log($"🧱 Nouveau tilemap instancié pour région {msg.region} à position {worldPos}");
        }

        Debug.Log($"🧱 Effacement de l'ancien contenu...");
        tilemap.ClearAllTiles();

        if (msg.tiles == null)
        {
            Debug.LogWarning($"⚠️ msg.tiles est null pour région {msg.region}");
            return;
        }

        foreach (var t in msg.tiles)
        {
            var tile = tileRegistry != null ? tileRegistry.Get(t.tileId) : null;
            if (tile == null)
            {
                Debug.LogWarning($"❌ tileId {t.tileId} introuvable dans tileRegistry");
                continue;
            }

            var localX = Mathf.FloorToInt(t.x - msg.region.x * RegionSize);
            var localY = Mathf.FloorToInt(t.y - msg.region.y * RegionSize);
            var cell = new Vector3Int(localX, localY, 0);
            tilemap.SetTile(cell, tile);
        }

        Debug.Log($"✅ Fin du rendu région {msg.region} ({msg.tiles.Count()} tiles placées)");
    }

}
