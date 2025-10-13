using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ServerMapManager : NetworkBehaviour
{
    public static ServerMapManager Instance;

    private readonly Dictionary<Vector2Int, RegionData> _regions = new();

    void Awake()
    {
        Debug.Log("👀 ServerMapManager.Awake() appelé !");
        Instance = this;

        LoadAllRegions();
        Debug.Log($"🌍 Fin du chargement : {_regions.Count} régions.");
    }


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        Debug.Log("🌐 Serveur prêt à envoyer les régions !");
        NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
        {
            Debug.Log($"👋 Client {clientId} connecté !");
            StartCoroutine(SendAllRegionsToClient(clientId));
        };
    }

    private IEnumerator SendAllRegionsToClient(ulong clientId)
    {
        Debug.Log($"📡 Envoi de toutes les régions sauvegardées ({_regions.Count}) au client {clientId}");
        foreach (var region in _regions.Keys)
        {
            SendRegionToClient(clientId, region);
            yield return null; // évite de saturer le buffer en envoyant tout d’un coup
        }
    }

    private IEnumerator SendInitialRegionDelayed(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f); // ou même 1s pour tester
        SendRegionToClient(clientId, Vector2Int.zero);
    }


    void OnApplicationQuit()
    {
        if (IsServer)
        {
            SaveAllRegions();
            Debug.Log("💾 Monde sauvegardé avant extinction !");
        }
    }

    public RegionData GetOrGenerateRegion(Vector2Int region)
    {
        if (!_regions.TryGetValue(region, out var data))
        {
            data = MapGenerator.GenerateRegion(region);
            _regions[region] = data;
            MapSerializer.SaveRegion(region, data);
        }
        return data;
    }

    public void SendRegionToClient(ulong clientId, Vector2Int region)
    {
        var data = GetOrGenerateRegion(region);
        Debug.Log($"🚀 Envoi région {region} au client {clientId} avec {Utils.SafeCount(data.tiles)} tiles et {Utils.SafeCount(data.objects)} objets");

        var msg = new RegionMessage
        {
            region = region,
            tiles = data.tiles,
            objects = data.objects
        };

        using var writer = new FastBufferWriter(4096, Unity.Collections.Allocator.Temp);
        writer.WriteValueSafe(msg);

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "ReceiveRegion",
            clientId,
            writer,
            NetworkDelivery.ReliableFragmentedSequenced
        );
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRegionServerRpc(Vector2Int region, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;

        if (!_regions.ContainsKey(region))
        {
            Debug.Log($"🧱 Région {region} absente — génération côté serveur !");
            var data = MapGenerator.GenerateRegion(region);
            _regions[region] = data;
            MapSerializer.SaveRegion(region, data);
        }
        else
        {
            Debug.Log($"📦 Région {region} déjà en mémoire — envoi direct.");
        }

        SendRegionToClient(senderId, region);
    }

    public void SaveAllRegions()
    {
        var data = new SaveData();

        foreach (var kv in _regions)
        {
            var save = new RegionSave
            {
                region = kv.Key,
                tiles = kv.Value.tiles,
                objects = kv.Value.objects
            };
            data.regions.Add(save);
        }

        SaveSystem.Save(data);
    }

    public void LoadAllRegions()
    {
        var data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning("⚠️ Aucune donnée trouvée dans SaveSystem.Load()");
            data = new SaveData();
        }

        if (data.regions == null)
        {
            Debug.LogWarning("⚠️ Liste des régions vide dans le fichier — création d’une nouvelle liste.");
            data.regions = new List<RegionSave>();
        }

        _regions.Clear();

        foreach (var save in data.regions)
        {
            var regionData = new RegionData
            {
                tiles = save.tiles,
                objects = save.objects
            };
            _regions[save.region] = regionData;
        }

        Debug.Log($"📦 Chargement terminé : {data.regions.Count} régions en mémoire.");
    }

}
