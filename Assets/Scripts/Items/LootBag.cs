using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Représente un loot au sol contenant plusieurs équipements.
/// Quand un joueur marche dessus, tous les items sont donnés.
/// </summary>
[RequireComponent(typeof(NetworkObject), typeof(Collider))]
public class LootBag : NetworkBehaviour
{
    [SerializeField] private List<string> ItemIds = new(); // Liste des noms d'items à donner
    [SerializeField] private float RespawnTime = 0f; // 0 = pas de respawn
    [SerializeField] private bool DestroyAfterLoot = true;

    private bool IsLooted = false;

    private void OnTriggerEnter2D(Collider2D p_Other)
    {
        if (!IsServer || IsLooted)
            return;

        PlayerInventory v_Inventory = p_Other.GetComponent<PlayerInventory>();
        if (v_Inventory == null)
            return;

        // ✅ Donne tous les items
        foreach (var v_ItemName in ItemIds)
        {
            v_Inventory.AddItemInternal(v_ItemName);
        }

        IsLooted = true;

        if (DestroyAfterLoot)
        {
            if (RespawnTime <= 0)
                DespawnLoot();
            else
                StartCoroutine(RespawnAfterDelay(RespawnTime));
        }
    }

    private void DespawnLoot()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
        else
            Destroy(gameObject);
    }

    private System.Collections.IEnumerator RespawnAfterDelay(float p_Delay)
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(p_Delay);
        IsLooted = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Définit une liste d’items pour ce loot.
    /// </summary>
    public void SetItems(List<string> p_Items)
    {
        ItemIds = p_Items;
    }
}