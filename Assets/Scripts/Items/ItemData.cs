using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Equipment")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public string ItemId;
    public Rarity Rarity;

    [Tooltip("Liste des bonus de stats que cet �quipement conf�re")]
    public EquipmentStatRange[] StatBonuses;
}
