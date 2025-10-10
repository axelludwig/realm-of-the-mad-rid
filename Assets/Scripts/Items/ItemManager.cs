using UnityEngine;

public class ItemManager : BaseSingleton<ItemManager>
{
    [Header("R�f�rences")]
    [SerializeField] private ItemDatabase ItemDatabase;

    public ItemDatabase GetDatabase() => ItemDatabase;


    /// <summary>
    /// G�n�re une instance d'item c�t� serveur � partir de son nom.
    /// </summary>
    public ItemInstance GenerateItemById(string p_ItemId)
    {
        if (!IsServer)
        {
            Debug.LogWarning("ItemManager.GenerateItemByName() doit �tre appel� c�t� serveur !");
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
