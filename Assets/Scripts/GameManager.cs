using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject LootBagPrefab;

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

            SpawnLoot(new Vector3(3f, 3f, 0f));
            SpawnLoot(new Vector3(3f, -3f, 0f));
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!PlayersIds.Contains(clientId))
            PlayersIds.Add(clientId);

        GiveStarterItemToPlayer(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (PlayersIds.Contains(clientId))
            PlayersIds.Remove(clientId);
    }

    /// <summary>
    /// Donne un item de départ ("Couteau rouillé") au joueur à la connexion.
    /// </summary>
    private void GiveStarterItemToPlayer(ulong p_ClientId)
    {
        GameObject v_Player = GetPlayerByClientId(p_ClientId);
        if (v_Player == null)
        {
            Debug.LogWarning($"Impossible de donner un item : joueur {p_ClientId} introuvable.");
            return;
        }

        PlayerInventory v_Inventory = v_Player.GetComponent<PlayerInventory>();
        if (v_Inventory == null)
        {
            Debug.LogWarning($"Le joueur {p_ClientId} n’a pas de PlayerInventory !");
            return;
        }

        // ✅ Génère l’item côté serveur
        var v_ItemData = ItemManager.Instance.GenerateNetworkItem("Couteau rouillé");

        // ✅ Donne-le au joueur via son ClientRpc
        v_Inventory.GiveItemClientRpc(v_ItemData, p_ClientId);

        Debug.Log($"🗡️ Donné 'Couteau rouillé' au joueur {p_ClientId}");
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
    [ServerRpc(RequireOwnership = false)]
    public void SpawnEnemyServerRpc(Vector2 p_Pos)
    {
        GameObject v_Enemy = Instantiate(EnemyPrefab, p_Pos, Quaternion.identity);
        var v_NetObj = v_Enemy.GetComponent<NetworkObject>();
        v_NetObj.Spawn(true);

        AIIds.Add(v_NetObj.NetworkObjectId);
    }

    private void SpawnLoot(Vector2 p_Pos)
    {
        GameObject v_Loot = Instantiate(LootBagPrefab, p_Pos, Quaternion.identity);
        LootBag v_LootBag = v_Loot.GetComponent<LootBag>();

        // ✅ Exemple : plusieurs équipements dans le même loot
        v_LootBag.SetItems(new List<string> { "Couteau rouillé"});

        v_Loot.GetComponent<NetworkObject>().Spawn(true);
        Debug.Log("💰 LootBag multi-items spawnée au sol !");
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
