using Unity.Netcode;
using UnityEngine;

public class PlayerRegionTracker : NetworkBehaviour
{
    private Vector2Int lastRegion;

    void Update()
    {
        if (!IsOwner) return;

        var playerPos = transform.position;
        var currentRegion = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / 8),
            Mathf.FloorToInt(playerPos.y / 8)
        );

        if (currentRegion != lastRegion)
        {
            lastRegion = currentRegion;
            RequestRegionServerRpc(currentRegion);
            Debug.Log($"📨 Envoi RequestRegionServerRpc vers le serveur pour région {currentRegion}");
        }

    }

    [ServerRpc]
    void RequestRegionServerRpc(Vector2Int region)
    {
        // Appelle côté serveur ServerMapManager
        ServerMapManager.Instance.SendRegionToClient(OwnerClientId, region);
    }
}
