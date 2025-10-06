using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] private GameObject v_EnemyPrefab;

    public NetworkList<ulong> PlayersIds;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
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

    protected override void Awake()
    {
        base.Awake();
        PlayersIds = new NetworkList<ulong>();
    }

    /// <summary>
    /// Fait apparaître un ennemi sur le serveur à une position donnée.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyServerRpc(Vector2 p_Pos)
    {
        GameObject v_Enemy = Instantiate(v_EnemyPrefab, p_Pos, Quaternion.identity);
        v_Enemy.GetComponent<NetworkObject>().Spawn(true);
    }

    /// <summary>
    /// Permet de récupérer la liste des objets Player actuellement présents dans la scène.
    /// </summary>
    public List<Player> GetPlayerObjects()
    {
        return FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
    }
}
