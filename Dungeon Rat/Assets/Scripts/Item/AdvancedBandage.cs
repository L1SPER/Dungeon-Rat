using UnityEngine;

[CreateAssetMenu(fileName = "AdvancedBandage", menuName = "Item/ConsumableItem/AdvancedBandage", order = 2)]
public class AdvancedBandage : ConsumableItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.itemName = "Advanced Bandage";
        this.maxStackSize = 5;
        this.price = 100;
        this.healAmount = 25;
    }
}
