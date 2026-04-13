using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    ItemData item;
    ShopUIManager manager;
    bool isSetup = false;

    public void Setup(ItemData itemData, ShopUIManager m)
    {
        item = itemData;
        manager = m;
        itemIcon.sprite = item.icon;
        isSetup = true;
        Debug.Log("Setup tamamlandı: " + item.itemName + " | GameObject: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Tıklanan obje: " + gameObject.name + " | isSetup: " + isSetup);

        if (!isSetup)
        {
            Debug.LogError("Setup yapılmamış! Bu obje: " + gameObject.name);
            return;
        }

        manager.OnItemSelected(item);
    }
}