using UnityEngine;

[System.Serializable]
public abstract class ItemData : ScriptableObject
{
    [Header("General")]
    public string itemName;
    public int itemID;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Item Type")]
    public ItemType itemType;
    public ItemRarity itemRarity;

    [Header("Stacking")]
    public int maxStackSize;
    public bool IsStackable => maxStackSize>1;
}
