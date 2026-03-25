using UnityEngine;

public class MaterialItemData : ItemData
{
    protected virtual void Awake()
    {
        itemType= ItemType.Material;
    }
}
