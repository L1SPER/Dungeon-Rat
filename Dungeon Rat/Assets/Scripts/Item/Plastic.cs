using UnityEngine;

[CreateAssetMenu(fileName = "Plastic", menuName = "Item/Material/Plastic", order = 5)]
public class Plastic : MaterialItemData
{
    protected override void Awake()
    {
        base.Awake();
        this.maxStackSize = 50;
    }   
}
