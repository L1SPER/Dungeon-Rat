using UnityEngine;

[CreateAssetMenu(fileName = "Metal", menuName = "Item/Material/Metal", order = 1)]
public class Metal: MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 50;
    }
}
