using UnityEngine;

[CreateAssetMenu(fileName = "Coin", menuName = "Item/Material/Coin", order = 2)]
public class Coin : MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 1000;
        this.isCurrency = true;
    }
}
