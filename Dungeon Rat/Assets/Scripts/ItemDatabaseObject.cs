using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "ItemDatabaseObject")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemData[] items;
    public Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].itemID = i;
            if (!items[i].isStackable)
            {
                items[i].maxStackSize = 1;
            }
            itemDictionary.Add(i, items[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        itemDictionary= new Dictionary<int, ItemData>();
    }
}
