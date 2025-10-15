using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class PlayerInventory : NetworkBehaviour
{
    private Entity entity;
    private NetworkList<NetItem> _items = new NetworkList<NetItem>(
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Server);

    public NetworkList<NetItem> GetInventory() => _items;

    public override void OnNetworkSpawn()
    {
        entity = GetComponent<Entity>();
        if (IsClient)
        {
            _items.OnListChanged += OnInventoryChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (_items != null)
            _items.OnListChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged(NetworkListEvent<NetItem> e)
    {
        Debug.Log($"Inventaire maj pour {OwnerClientId} : {_items.Count} items");
        List<NetItem> newItems = new();
        List<NetItem> removedItems = new();

        switch (e.Type)
        {
            case NetworkListEvent<NetItem>.EventType.Add:
                newItems.Add(e.Value);
                break;

            case NetworkListEvent<NetItem>.EventType.Remove:
                removedItems.Add(e.Value);
                break;

            case NetworkListEvent<NetItem>.EventType.Insert:
                newItems.Add(e.Value);
                break;

            case NetworkListEvent<NetItem>.EventType.Value:
                // Un élément a été remplacé : l’ancien est "removed", le nouveau "added"
                removedItems.Add(e.PreviousValue);
                newItems.Add(e.Value);
                break;
        }

        foreach (NetItem item in newItems)
        {
            AddItemStats(item);
        }

        foreach (NetItem item in removedItems)
        {
            RemoveItemStats(item);
        }
    }



    private void AddItemStats(NetItem item)
    {
        foreach (var itemStat in item.Stats)
        {
            var entityStat = GetEntityStatFromItemStat(itemStat);
            if (entityStat.boostType == BoostType.Flat)
            {
                entityStat.stat.AddBonus(itemStat.Value);
            }
            else
            {
                entityStat.stat.AddMultiplier(itemStat.Value / 100f);
            }
        }
    }

    private void RemoveItemStats(NetItem item)
    {
        foreach (var itemStat in item.Stats)
        {
            var entityStat = GetEntityStatFromItemStat(itemStat);
            if (entityStat.boostType == BoostType.Flat)
            {
                entityStat.stat.RemoveBonus(itemStat.Value);
            }
            else
            {
                entityStat.stat.RemoveMultiplier(itemStat.Value / 100f);
            }
        }
    }

    private (BoostType boostType, Stat stat) GetEntityStatFromItemStat(ItemStat itemStat)
    {
        return itemStat.Type switch
        {
            StatType.StrengthFlat => (BoostType.Flat, entity.Stats.Strength),
            StatType.StrengthPercent => (BoostType.Percent, entity.Stats.Strength),

            StatType.IntelligenceFlat => (BoostType.Flat, entity.Stats.Intelligence),
            StatType.IntelligencePercent => (BoostType.Percent, entity.Stats.Intelligence),

            StatType.AuraRadiusFlat => (BoostType.Flat, entity.Stats.AuraRadius),
            StatType.AuraRadiusPercent => (BoostType.Percent, entity.Stats.AuraRadius),

            StatType.ArmourFlat => (BoostType.Flat, entity.Stats.Armour),
            StatType.ArmourPercent => (BoostType.Percent, entity.Stats.Armour),

            StatType.AttackSpeedFlat => (BoostType.Flat, entity.Stats.AttackSpeed),
            StatType.AttackSpeedPercent => (BoostType.Percent, entity.Stats.AttackSpeed),

            StatType.CooldownReductionFlat => (BoostType.Flat, entity.Stats.CooldownReduction),
            StatType.CooldownReductionPercent => (BoostType.Percent, entity.Stats.CooldownReduction),

            StatType.HealthFlat => (BoostType.Flat, entity.Stats.Health),
            StatType.HealthPercent => (BoostType.Percent, entity.Stats.Health),

            StatType.MovementSpeedFlat => (BoostType.Flat, entity.Stats.MovementSpeed),
            StatType.MovementSpeedPercent => (BoostType.Percent, entity.Stats.MovementSpeed),

            _ => throw new ArgumentOutOfRangeException(nameof(itemStat.Type), $"Type de stat inconnu : {itemStat.Type}")
        };
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

    public enum BoostType
    {
        Flat,
        Percent
    }
}
