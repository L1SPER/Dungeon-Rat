using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "ItemDatabaseObject")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemData[] items;
    public Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        RebuildDictionary();
    }

    public void RebuildDictionary()
    {
        itemDictionary = new Dictionary<int, ItemData>();

        if (items == null)
            return;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;

            items[i].itemID = i;

            if (items[i].maxStackSize < 1)
                items[i].maxStackSize = 1;

            itemDictionary[i] = items[i];
        }
    }

    public ItemData GetItemByID(int id)
    {
        if (itemDictionary == null || itemDictionary.Count == 0)
            RebuildDictionary();

        itemDictionary.TryGetValue(id, out ItemData result);
        return result;
    }
}
