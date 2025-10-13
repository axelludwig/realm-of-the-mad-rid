using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ClientRegionTracker : NetworkBehaviour
{
    private Vector2Int _currentRegion;
    private HashSet<Vector2Int> _loadedRegions = new();

    private const int RegionSize = 8;

    void Update()
    {
        if (!IsOwner) return; // on ne suit que le joueur local
        if (ServerMapManager.Instance == null) return;

        Vector2 pos = transform.position;
        Vector2Int region = new(Mathf.FloorToInt(pos.x / RegionSize), Mathf.FloorToInt(pos.y / RegionSize));

        if (region != _currentRegion)
        {
            _currentRegion = region;
            TryRequestRegion(region);
        }
    }

    void TryRequestRegion(Vector2Int region)
    {
        if (_loadedRegions.Contains(region)) return;
        _loadedRegions.Add(region);

        Debug.Log($"Demande de la région {region} au serveur...");
        ServerMapManager.Instance.RequestRegionServerRpc(region);
    }
}
