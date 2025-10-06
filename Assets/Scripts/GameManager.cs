using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] private GameObject v_EnemyPrefab;

    // ✅ Instanciation directe (obligatoire pour Netcode)
    public NetworkList<ulong> PlayersIds = new();
    public NetworkList<ulong> AIIds = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            PlayersIds.Clear();
            AIIds.Clear();

            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!PlayersIds.Contains(clientId))
            PlayersIds.Add(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (PlayersIds.Contains(clientId))
            PlayersIds.Remove(clientId);
    }

    /// <summary>
    /// Fait apparaître une IA sur le serveur à une position donnée.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyServerRpc(Vector2 p_Pos)
    {
        GameObject v_Enemy = Instantiate(v_EnemyPrefab, p_Pos, Quaternion.identity);
        var v_NetObj = v_Enemy.GetComponent<NetworkObject>();
        v_NetObj.Spawn(true);

        AIIds.Add(v_NetObj.NetworkObjectId);
    }

    /// <summary>
    /// Retourne la liste des joueurs actifs.
    /// </summary>
    public List<Player> GetPlayerObjects()
    {
        List<Player> v_Result = new();

        foreach (ulong v_ClientId in PlayersIds)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(v_ClientId, out var v_Client))
            {
                var v_PlayerObj = v_Client.PlayerObject;
                if (v_PlayerObj != null)
                {
                    var v_Player = v_PlayerObj.GetComponent<Player>();
                    if (v_Player != null)
                        v_Result.Add(v_Player);
                }
            }
        }

        return v_Result;
    }

    /// <summary>
    /// Retourne la liste des IA actives.
    /// </summary>
    public List<Enemy> GetAIObjects()
    {
        List<Enemy> v_Result = new();

        foreach (ulong v_ObjId in AIIds)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(v_ObjId, out var v_Obj))
            {
                var v_Enemy = v_Obj.GetComponent<Enemy>();
                if (v_Enemy != null)
                    v_Result.Add(v_Enemy);
            }
        }

        return v_Result;
    }
}
