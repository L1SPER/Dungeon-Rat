using UnityEngine;

[CreateAssetMenu(fileName = "Wood", menuName = "Item/Material/Wood", order = 4)]
public class Wood : MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 50;
    }
}
