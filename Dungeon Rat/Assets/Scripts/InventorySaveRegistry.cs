using System.Collections.Generic;
using UnityEngine;

public class InventorySaveRegistry : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GeneralInventoryObject generalInventoryObject;
    [SerializeField] private RatInventoryObject ratInventoryObject;
    [SerializeField] private LootInventoryObject lootInventoryObject;

    [Header("Equipment")]
    [SerializeField] private EquipmentInventoryObject[] equipmentInventoryObjects;

    public IEnumerable<InventoryObjectReference> GetAllInventories()
    {
        if (generalInventoryObject != null)
            yield return new InventoryObjectReference("general_inventory", generalInventoryObject.itemDatabase, generalInventoryObject.inventory);

        if (ratInventoryObject != null)
            yield return new InventoryObjectReference("rat_inventory", ratInventoryObject.itemDatabase, ratInventoryObject.inventory);

        if (lootInventoryObject != null)
            yield return new InventoryObjectReference("loot_inventory", lootInventoryObject.itemDatabase, lootInventoryObject.inventory);

        if (equipmentInventoryObjects == null)
            yield break;

        for (int i = 0; i < equipmentInventoryObjects.Length; i++)
        {
            EquipmentInventoryObject equipmentObject = equipmentInventoryObjects[i];
            if (equipmentObject == null)
                continue;

            yield return new InventoryObjectReference($"equipment_inventory_{i}", equipmentObject.itemDatabase, equipmentObject.inventory);
        }
    }
}

public struct InventoryObjectReference
{
    public string Key;
    public ItemDatabaseObject ItemDatabase;
    public InventoryBase Inventory;

    public InventoryObjectReference(string key, ItemDatabaseObject itemDatabase, InventoryBase inventory)
    {
        Key = key;
        ItemDatabase = itemDatabase;
        Inventory = inventory;
    }
}