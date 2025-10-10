using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerInventory : NetworkBehaviour
{
    private  NetworkList<NetItem> _items = new NetworkList<NetItem>(
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Server);

    public NetworkList<NetItem> GetInventory() => _items;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            _items.OnListChanged += OnInventoryChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (_items != null)
            _items.OnListChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged(NetworkListEvent<NetItem> e)
    {
        Debug.Log($"Inventaire maj pour {OwnerClientId} : {_items.Count} items");
        // -> mettre à jour l’UI ici
    }

    [ServerRpc]
    public void AddItemServerRpc(string p_ItemName)
    {
        AddItemInternal(p_ItemName);
    }

    /// <summary>
    /// Ajoute un item à l’inventaire du joueur.
    /// </summary>
    /// <param name="p_ItemName"></param>
    public void AddItemInternal(string p_ItemName)
    {
        if (!IsServer)
        {
            Debug.LogWarning("AddItemInternal doit être appelé côté serveur !");
            return;
        }

        if (!IsSpawned)
        {
            Debug.LogWarning($"Tentative d’ajouter un item avant le spawn du NetworkObject (Player {OwnerClientId})");
            return;
        }


        var db = ItemManager.Instance.GetDatabase();
        var item = ItemManager.Instance.GenerateItemByName(p_ItemName);
        if (item == null) return;

        NetItem ni = default;
        ni.ItemId = GetItemId(item.Data);
        ni.Name = item.Data.ItemName;

        var ordered = item.FinalStats.OrderBy(k => k.Key);
        foreach (var kv in ordered)
        {
            if (ni.Stats.Length < ni.Stats.Capacity)
                ni.Stats.Add(new ItemStat { Type = kv.Key, Value = kv.Value });
            else
                Debug.LogWarning($"Trop de stats pour l’item {p_ItemName} (capacité FixedList atteinte).");
        }

        Debug.Log($"[{name}] AddItemInternal pour joueur {OwnerClientId}, " +
          $"IsSpawned={IsSpawned}, NetworkObject.IsSpawned={NetworkObject.IsSpawned}, " +
          $"IsServer={IsServer}, NetworkManager.Singleton.IsServer={NetworkManager.Singleton.IsServer}");


        _items.Add(ni);
    }

    private int GetItemId(ItemData data)
    {
        var db = ItemManager.Instance.GetDatabase();
        for (int i = 0; i < db.AllItems.Length; i++)
            if (db.AllItems[i] == data) return i;
        return -1;
    }
}
