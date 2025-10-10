using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    private List<ItemInstance> _items = new();

    public IReadOnlyList<ItemInstance> Items => _items;

    /// <summary>
    /// Appelé côté client pour demander un item au serveur.
    /// </summary>
    public void RequestItem(string p_ItemName)
    {
        if (!IsOwner) return; // sécurité
        RequestItemServerRpc(p_ItemName);
    }

    /// <summary>
    /// Exécuté sur le serveur quand un client demande un item.
    /// </summary>
    [ServerRpc]
    private void RequestItemServerRpc(string p_ItemName, ServerRpcParams p_Params = default)
    {
        ulong v_SenderClientId = p_Params.Receive.SenderClientId;

        var v_ItemData = ItemManager.Instance.GenerateNetworkItem(p_ItemName);

        // Envoie le résultat à tous les clients, avec l’ID du destinataire
        GiveItemClientRpc(v_ItemData, v_SenderClientId);
    }

    /// <summary>
    /// Reçoit l’item côté client.
    /// </summary>
    [ClientRpc]
    public void GiveItemClientRpc(ItemNetworkData p_ItemData, ulong p_ClientId)
    {
        // Vérifie que c’est bien nous
        if (NetworkManager.Singleton.LocalClientId != p_ClientId)
            return;

        var v_Item = ItemInstance.FromNetworkData(p_ItemData, ItemManager.Instance.GetDatabase());
        if (v_Item != null)
            _items.Add(v_Item);

        Debug.Log($"🎁 Joueur {p_ClientId} a reçu {v_Item}");
    }
}
