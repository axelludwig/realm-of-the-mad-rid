using UnityEngine;

public class ItemManager : BaseSingleton<ItemManager>
{
    [Header("R�f�rences")]
    [SerializeField] private ItemDatabase ItemDatabase;

    public ItemDatabase GetDatabase() => ItemDatabase;


    /// <summary>
    /// G�n�re une instance d'item c�t� serveur � partir de son nom.
    /// </summary>
    public ItemInstance GenerateItemByName(string p_ItemName)
    {
        if (!IsServer)
        {
            Debug.LogWarning("ItemManager.GenerateItemByName() doit �tre appel� c�t� serveur !");
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
    /// G�n�re un item et le renvoie sous forme de donn�es r�seau.
    /// </summary>
    public ItemNetworkData GenerateNetworkItem(string p_ItemName)
    {
        var v_Instance = GenerateItemByName(p_ItemName);
        return v_Instance?.ToNetworkData() ?? default;
    }
}
