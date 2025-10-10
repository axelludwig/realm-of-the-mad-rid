using UnityEngine;

public class ItemManager : BaseSingleton<ItemManager>
{
    [Header("Références")]
    [SerializeField] private ItemDatabase ItemDatabase;

    public ItemDatabase GetDatabase() => ItemDatabase;


    /// <summary>
    /// Génère une instance d'item côté serveur à partir de son nom.
    /// </summary>
    public ItemInstance GenerateItemByName(string p_ItemName)
    {
        if (!IsServer)
        {
            Debug.LogWarning("ItemManager.GenerateItemByName() doit être appelé côté serveur !");
            return null;
        }

        ItemData v_ItemData = ItemDatabase.GetItemByName(p_ItemName);
        if (v_ItemData == null)
        {
            Debug.LogError($"Item '{p_ItemName}' introuvable dans la base !");
            return null;
        }

        return new ItemInstance(v_ItemData);
    }

    /// <summary>
    /// Génère un item et le renvoie sous forme de données réseau.
    /// </summary>
    public ItemNetworkData GenerateNetworkItem(string p_ItemName)
    {
        var v_Instance = GenerateItemByName(p_ItemName);
        return v_Instance?.ToNetworkData() ?? default;
    }
}
