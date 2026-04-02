using UnityEngine;

[CreateAssetMenu(fileName = "BasicMaterial", menuName = "Item/Material/BasicMaterial", order = 1)]
public class BasicMaterial: MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 50;
    }
}
