using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/Database")]
public class ItemDatabase : ScriptableObject
{
    [Tooltip("Liste de tous les items disponibles dans le jeu")]
    public ItemData[] AllItems;

    /// <summary>
    /// Recherche un item par son nom.
    /// </summary>
    public ItemData GetItemByName(string p_Name)
    {
        foreach (var v_Item in AllItems)
        {
            if (v_Item.ItemName == p_Name)
                return v_Item;
        }
        Debug.LogWarning($"Item introuvable dans la database : {p_Name}");
        return null;
    }

    /// <summary>
    /// (Optionnel) Recherche par ID si tu veux éviter les strings.
    /// </summary>
    public ItemData GetItemById(int p_Id)
    {
        if (p_Id < 0 || p_Id >= AllItems.Length)
        {
            Debug.LogWarning($"ID d'item invalide : {p_Id}");
            return null;
        }
        return AllItems[p_Id];
    }
}