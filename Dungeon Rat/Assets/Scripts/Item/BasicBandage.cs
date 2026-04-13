using UnityEngine;

[CreateAssetMenu(fileName = "BasicBandage", menuName = "Item/ConsumableItem/BasicBandage", order = 1)]
public class BasicBandage : ConsumableItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.itemName = "Basic Bandage";
        this.maxStackSize = 5;
        this.price = 50;
        this.healAmount = 10;
    }
}
