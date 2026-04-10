using UnityEngine;

[CreateAssetMenu(fileName = "Cloth", menuName = "Item/Material/Cloth", order = 3)]
public class Cloth : MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 50;
    }
}