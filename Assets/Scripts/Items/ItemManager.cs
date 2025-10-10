using UnityEngine;

public class ItemManager : BaseSingleton<ItemManager>
{
    [Header("Références")]
    [SerializeField] private ItemDatabase ItemDatabase;

    public ItemDatabase GetDatabase() => ItemDatabase;


    /// <summary>
    /// Génère une instance d'item côté serveur à partir de son nom.
    /// </summary>
    public ItemInstance GenerateItemById(string p_ItemId)
    {
        if (!IsServer)
        {
            Debug.LogWarning("ItemManager.GenerateItemByName() doit être appelé côté serveur !");
            return null;
        }

        ItemData v_ItemData = ItemDatabase.GetItemById(p_ItemId);
        if (v_ItemData == null)
        {
            Debug.LogError($"Item '{p_ItemId}' introuvable dans la base !");
            return null;
        }

        return new ItemInstance(v_ItemData);
    }
}
