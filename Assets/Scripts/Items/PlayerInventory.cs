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
    public void AddItemServerRpc(string p_ItemId)
    {
        AddItemInternal(p_ItemId);
    }

    /// <summary>
    /// Ajoute un item au joueur (serveur uniquement).
    /// </summary>
    public void AddItemInternal(string p_ItemId)
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

        // ✅ Génération directe du NetItem complet
        var v_NetItem = ItemManager.Instance.GenerateNetItemById(p_ItemId);
        if (v_NetItem.Equals(default)) return;

        _items.Add(v_NetItem);
        Debug.Log($"🧳 Ajouté à l’inventaire du joueur {OwnerClientId} : {v_NetItem.Name} [#{v_NetItem.UniqueId}]");
    }
}
