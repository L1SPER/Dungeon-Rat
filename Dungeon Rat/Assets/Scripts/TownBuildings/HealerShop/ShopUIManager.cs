using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public List<ItemData> shopItems;

    public Transform slotParent;
    public GameObject shopItemPrefab;

    public ItemBuyMenu buyMenu;

    void Start()
    {
        CreateShopItems();
    }

    void CreateShopItems()
    {
        foreach (var item in shopItems)
        {
            GameObject go = Instantiate(shopItemPrefab, slotParent);

            Debug.Log("Prefab oluşturuldu: " + go.name);

            ShopItemUI ui = go.GetComponent<ShopItemUI>();

            if (ui == null)
            {
                Debug.LogError("ShopItemUI YOK!");
                continue;
            }

            Debug.Log("Setup çağrılıyor: " + item.itemName);

            ui.Setup(item, this);
        }
    }

    public void OnItemSelected(ItemData item)
    {
        buyMenu.Open(item);
    }
}