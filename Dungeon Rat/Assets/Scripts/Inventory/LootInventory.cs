using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootInventory : ContainerInventoryBase
{
    [System.Serializable]
    public class RarityWeight
    {
        public ItemRarity rarity;
        [Range(0, 100)] public int weight;
    }

    [System.Serializable]
    public class TierLootTable
    {
        public DungeonTier dungeonTier;
        public RarityWeight[] rarityWeights;
    }

    [Header("Dungeon Loot Tables")]
    [SerializeField]
    private TierLootTable[] tierLootTables =
    {
        new TierLootTable
        {
            dungeonTier = DungeonTier.Common,
            rarityWeights = new[]
            {
                new RarityWeight { rarity = ItemRarity.Common, weight = 90 },
                new RarityWeight { rarity = ItemRarity.Uncommon, weight = 10 }
            }
        },
        new TierLootTable
        {
            dungeonTier = DungeonTier.Uncommon,
            rarityWeights = new[]
            {
                new RarityWeight { rarity = ItemRarity.Common, weight = 60 },
                new RarityWeight { rarity = ItemRarity.Uncommon, weight = 30 },
                new RarityWeight { rarity = ItemRarity.Rare, weight = 10 }
            }
        },
        new TierLootTable
        {
            dungeonTier = DungeonTier.Rare,
            rarityWeights = new[]
            {
                new RarityWeight { rarity = ItemRarity.Uncommon, weight = 55 },
                new RarityWeight { rarity = ItemRarity.Rare, weight = 35 },
                new RarityWeight { rarity = ItemRarity.Epic, weight = 10 }
            }
        },
        new TierLootTable
        {
            dungeonTier = DungeonTier.Epic,
            rarityWeights = new[]
            {
                new RarityWeight { rarity = ItemRarity.Rare, weight = 55 },
                new RarityWeight { rarity = ItemRarity.Epic, weight = 35 },
                new RarityWeight { rarity = ItemRarity.Legendary, weight = 10 }
            }
        },
        new TierLootTable
        {
            dungeonTier = DungeonTier.Legendary,
            rarityWeights = new[]
            {
                new RarityWeight { rarity = ItemRarity.Epic, weight = 60 },
                new RarityWeight { rarity = ItemRarity.Legendary, weight = 40 }
            }
        }
    };

    public void ConfigureLootInventory()
    {
        Configure(5, 5, false);
    }

    public void GenerateDungeonLoot(ItemData[] allItems, DungeonTier dungeonTier, int itemCount = 5)
    {
        Clear();

        if (allItems == null || allItems.Length == 0)
            return;

        itemCount = Mathf.Clamp(itemCount, 0, currentCapacity);

        List<ItemData> availableItems = new List<ItemData>();
        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i] != null)
                availableItems.Add(allItems[i]);
        }

        if (availableItems.Count == 0)
            return;

        HashSet<ItemData> usedItems = new HashSet<ItemData>();

        for (int i = 0; i < itemCount; i++)
        {
            ItemRarity rolledRarity = RollRarity(dungeonTier);

            List<ItemData> rarityPool = new List<ItemData>();
            for (int j = 0; j < availableItems.Count; j++)
            {
                ItemData item = availableItems[j];

                if (item == null || usedItems.Contains(item))
                    continue;

                if (item.itemRarity == rolledRarity)
                    rarityPool.Add(item);
            }

            // O rarity'de item yoksa fallback
            if (rarityPool.Count == 0)
            {
                for (int j = 0; j < availableItems.Count; j++)
                {
                    ItemData item = availableItems[j];

                    if (item == null || usedItems.Contains(item))
                        continue;

                    rarityPool.Add(item);
                }
            }

            if (rarityPool.Count == 0)
                break;

            ItemData selectedItem = rarityPool[Random.Range(0, rarityPool.Count)];
            int amount = selectedItem.IsStackable
                ? Random.Range(1, selectedItem.maxStackSize + 1)
                : 1;

            AddItem(selectedItem, amount);
            usedItems.Add(selectedItem);
        }
    }

    private ItemRarity RollRarity(DungeonTier dungeonTier)
    {
        TierLootTable lootTable = GetLootTable(dungeonTier);

        if (lootTable == null || lootTable.rarityWeights == null || lootTable.rarityWeights.Length == 0)
            return ItemRarity.Common;

        int totalWeight = 0;
        for (int i = 0; i < lootTable.rarityWeights.Length; i++)
        {
            if (lootTable.rarityWeights[i] != null)
                totalWeight += Mathf.Max(0, lootTable.rarityWeights[i].weight);
        }

        if (totalWeight <= 0)
            return ItemRarity.Common;

        int roll = Random.Range(0, totalWeight);
        int current = 0;

        for (int i = 0; i < lootTable.rarityWeights.Length; i++)
        {
            RarityWeight weightEntry = lootTable.rarityWeights[i];
            if (weightEntry == null)
                continue;

            current += Mathf.Max(0, weightEntry.weight);

            if (roll < current)
                return weightEntry.rarity;
        }

        return ItemRarity.Common;
    }

    private TierLootTable GetLootTable(DungeonTier dungeonTier)
    {
        if (tierLootTables == null || tierLootTables.Length == 0)
            return null;

        for (int i = 0; i < tierLootTables.Length; i++)
        {
            if (tierLootTables[i] != null && tierLootTables[i].dungeonTier == dungeonTier)
                return tierLootTables[i];
        }

        return null;
    }
}