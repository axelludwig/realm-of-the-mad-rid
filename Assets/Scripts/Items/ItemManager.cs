using Unity.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Gère la génération et l’attribution des items dans le jeu.
/// </summary>
public class ItemManager : BaseSingleton<ItemManager>
{
    [Header("Références")]
    [SerializeField] private ItemDatabase ItemDatabase;

    private int v_NextUniqueId = 1; // compteur interne serveur

    public ItemDatabase GetDatabase() => ItemDatabase;

    /// <summary>
    /// Génère un NetItem complet à partir de son ID (défini dans ItemDatabase).
    /// </summary>
    public NetItem GenerateNetItemById(string p_ItemId)
    {
        if (!IsServer)
        {
            Debug.LogWarning("ItemManager.GenerateNetItemById() doit être appelé côté serveur !");
            return default;
        }

        ItemData v_ItemData = ItemDatabase.GetItemById(p_ItemId);
        if (v_ItemData == null)
        {
            Debug.LogError($"Item '{p_ItemId}' introuvable dans la base !");
            return default;
        }

        // ✅ Création de l'item réseau
        NetItem v_Item = default;
        v_Item.UniqueId = v_NextUniqueId++;
        v_Item.Name = v_ItemData.ItemName;
        v_Item.GlobalId = p_ItemId;
        v_Item.Stats = new FixedList128Bytes<ItemStat>();

        // Génération des stats aléatoires
        var v_OrderedStats = v_ItemData.StatBonuses.OrderBy(s => s.StatType);
        foreach (var v_Stat in v_OrderedStats)
        {
            float v_Value = v_Stat.GetRandomValue();
            if (v_Item.Stats.Length < v_Item.Stats.Capacity)
            {
                v_Item.Stats.Add(new ItemStat { Type = v_Stat.StatType, Value = v_Value });
            }
            else
            {
                Debug.LogWarning($"Trop de stats pour l’item {p_ItemId} (capacité FixedList atteinte).");
                break;
            }
        }

        Debug.Log($"Généré {v_Item.Name} (GlobalId={p_ItemId}, UniqueId={v_Item.UniqueId})");
        return v_Item;
    }
}
