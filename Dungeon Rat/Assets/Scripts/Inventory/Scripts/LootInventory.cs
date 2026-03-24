using UnityEngine;

[System.Serializable]
public class LootInventory : ContainerInventoryBase
{
    public void ConfigureLootInventory()
    {
        Configure(5, 5, false);
    }

    public void GenerateLoot(ItemData[] possibleItems, int minItemCount, int maxItemCount)
    {
        Clear();

        if (possibleItems == null || possibleItems.Length == 0)
            return;

        int lootCount = Random.Range(minItemCount, maxItemCount + 1);
        lootCount = Mathf.Min(lootCount, currentCapacity);

        for (int i = 0; i < lootCount; i++)
        {
            ItemData randomItem = possibleItems[Random.Range(0, possibleItems.Length)];
            if (randomItem == null)
                continue;

            int amount = 1;

            if (randomItem.IsStackable)
                amount = Random.Range(1, randomItem.maxStackSize + 1);

            AddItem(randomItem, amount);
        }
    }
}