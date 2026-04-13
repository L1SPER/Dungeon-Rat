using System;
using UnityEngine;

public class AfterDungeonCanvasUI : MonoBehaviour
{
    [SerializeField] private RatInventoryUI ratInventoryUI;
    [SerializeField] private InventoryUI generalInventoryUI;

    [SerializeField] private RatInventoryObject ratInventoryObject;
    [SerializeField] private GeneralInventoryObject generalInventoryObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        ratInventoryUI?.RefreshUI();
        generalInventoryUI.RefreshUI();
    }

    public void TransferLootToGeneralInventory()
    {
        RatInventory ratInventory = ratInventoryObject.inventory;
        GeneralInventory generalInventory = generalInventoryObject.inventory;

        if (ratInventory == null || generalInventory == null)
        {
            Debug.LogError("Inventory referansı eksik.");
            return;
        }

        bool anyMoved = false;

        for (int i = 0; i < ratInventory.Slots.Length; i++)
        {
            InventorySlot ratSlot = ratInventory.Slots[i];

            if (ratSlot == null || ratSlot.IsEmpty())
                continue;

            while (!ratSlot.IsEmpty())
            {
                int beforeAmount = ratSlot.item.amount;
                ItemData beforeItem = ratSlot.item.itemData;

                bool moved = InventoryTransferUtility.MoveItemToFirstValidSlot(ratSlot, generalInventory);

                if (!moved)
                    break;

                anyMoved = true;

                bool noProgress =
                    !ratSlot.IsEmpty() &&
                    ratSlot.item.itemData == beforeItem &&
                    ratSlot.item.amount == beforeAmount;

                if (noProgress)
                {
                    Debug.LogWarning("Transfer durdu: item taşınmış görünse de slot değişmedi. Olası swap/merge hatası.");
                    break;
                }
            }
        }

        if (!anyMoved)
            Debug.Log("Hiçbir item taşınamadı.");

        RefreshUI();
    }

    public void OnClickExitDungeon()
    {
        TransferLootToGeneralInventory();
        FloatingCombatTextManager.Instance?.ClearAllFloatingTexts();
        SaveSystemManager.Instance?.SaveGame();
        GameSceneManager.Instance.LoadScene("Dungeon Entrance");
    }
}
