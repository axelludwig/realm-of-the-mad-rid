using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject EnemyBluePrefab;
    [SerializeField] private GameObject LootBagPrefab;

    // ✅ Instanciation directe (obligatoire pour Netcode)
    public NetworkList<ulong> PlayersIds = new();
    public NetworkList<ulong> AIIds = new();
    public List<List<ulong>> EnemyGroups = new List<List<ulong>>();


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
    /// Récupère le GameObject du joueur associé à un ClientId Netcode.
    /// </summary>
    public GameObject GetPlayerByClientId(ulong p_ClientId)
    {
        NetworkObject v_PlayerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(p_ClientId);

        if (v_PlayerObject == null)
        {
            Debug.LogWarning($"❌ Aucun joueur trouvé pour le clientId {p_ClientId}");
            return null;
        }

        return v_PlayerObject.gameObject;
    }

    /// <summary>
    /// Fait apparaître une IA sur le serveur à une position donnée.
    /// </summary>
    public ulong SpawnEnemy(GameObject prefab, Vector2 p_Pos)
    {
        GameObject v_Enemy = Instantiate(prefab, p_Pos, Quaternion.identity);
        var v_NetObj = v_Enemy.GetComponent<NetworkObject>();
        v_NetObj.Spawn(true);

        AIIds.Add(v_NetObj.NetworkObjectId);
        return v_NetObj.NetworkObjectId;
    }

    public void DespawnEnemy(NetworkObject networkEnemy)
    {
        var id = networkEnemy.NetworkObjectId;
        if (AIIds.Contains(id))
        {
            //Trouve l'ennemi dans la liste de groupes d'ennemis et le retire
            for (int i = 0; i < EnemyGroups.Count; i++)
            {
                if(EnemyGroups[i].Exists(elem => elem == id))
                {
                    EnemyGroups[i].Remove(id);
                }
                if(EnemyGroups[i].Count == 0)
                {
                    SpawnLoot("couteau-rouille", networkEnemy.transform.position);
                    EnemyGroups.RemoveAt(i);
                }
            }
            AIIds.Remove(id);
            networkEnemy.Despawn();
        }
    }

    /// <summary>
    /// Fait apparaître un groupe d'IA sur le serveur à une position donnée.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyGroupServerRpc(Vector2 p_Pos)
    {
        var enemiesToInstantiate = new List<GameObject>();
        enemiesToInstantiate.Add(EnemyPrefab);
        enemiesToInstantiate.Add(EnemyPrefab);
        enemiesToInstantiate.Add(EnemyBluePrefab);

        var enemyGroup = new List<ulong>();
        for (int i = 0; i < enemiesToInstantiate.Count; i++)
        {
            Vector2 spawnPos = new Vector2(p_Pos.x, p_Pos.y);
            spawnPos.x += i;
            spawnPos.y += i;
            enemyGroup.Add(SpawnEnemy(enemiesToInstantiate[i], spawnPos));
        }

        EnemyGroups.Add(enemyGroup);
    }

    private void SpawnLoot(string p_itemId, Vector2 p_Pos)
    {
        GameObject v_Loot = Instantiate(LootBagPrefab, p_Pos, Quaternion.identity);
        LootBag v_LootBag = v_Loot.GetComponent<LootBag>();

        // ✅ Exemple : plusieurs équipements dans le même loot
        v_LootBag.SetItems(new List<string> { p_itemId });

        v_Loot.GetComponent<NetworkObject>().Spawn(true);
        Debug.Log("💰 LootBag multi-items spawnée au sol !");
    }

    /// <summary>
    /// Retourne la liste des joueurs actifs.
    /// </summary>
    public List<Player> GetPlayerObjects()
    {
        if (NetworkManager.Singleton == null)
            return new List<Player>();

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
        if (NetworkManager.Singleton == null)
            return new List<Enemy>();

        List<Enemy> v_Result = new();

        foreach (ulong v_ObjId in AIIds)
        {

            var v_Enemy = GetAIObject(v_ObjId);
            if (v_Enemy != null)
                v_Result.Add(v_Enemy);
        }

        return v_Result;
    }

    public Enemy GetAIObject(ulong v_ObjId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(v_ObjId, out var v_Obj))
        {
            return v_Obj.GetComponent<Enemy>();
        }
        return null;
    }
}
