using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Item Database", menuName = "ItemDatabaseObject")]
public class ItemDatabaseObject : ScriptableObject
{
    public ItemData[] items;

    private Dictionary<int, ItemData> _idDictionary;
    private Dictionary<string, ItemData> _nameDictionary;

    private void OnEnable()
    {
        RebuildDictionary();
    }

    public void RebuildDictionary()
    {
        _idDictionary = new Dictionary<int, ItemData>();
        _nameDictionary = new Dictionary<string, ItemData>();

        if (items == null)
            return;

        for (int i = 0; i < items.Length; i++)
        {
            ItemData item = items[i];

            if (item == null)
                continue;

            // ID dictionary
            if (_idDictionary.ContainsKey(item.itemID))
            {
                Debug.LogError($"[ItemDatabase] Duplicate itemID {item.itemID} → '{item.name}'. " +
                               $"'Auto Assign IDs' context menu'sünü çalıştır.");
            }
            else
            {
                _idDictionary[item.itemID] = item;
            }

            // Asset adına göre dictionary (yedek arama)
            if (!_nameDictionary.ContainsKey(item.name))
                _nameDictionary[item.name] = item;
        }
    }

    /// <summary>
    /// Save/Load için kullanılan ana metod.
    /// </summary>
    public ItemData GetItemByID(int id)
    {
        if (_idDictionary == null || _idDictionary.Count == 0)
            RebuildDictionary();

        _idDictionary.TryGetValue(id, out ItemData result);
        return result;
    }

    /// <summary>
    /// Asset adına göre arama. Eski save dosyaları veya debug için.
    /// </summary>
    public ItemData GetItemByName(string assetName)
    {
        if (_nameDictionary == null || _nameDictionary.Count == 0)
            RebuildDictionary();

        _nameDictionary.TryGetValue(assetName, out ItemData result);
        return result;
    }

    /// <summary>
    /// Editor'da bir kez çalıştır. Sonra sıralamayı değiştirme, hep sona ekle.
    /// </summary>
    [ContextMenu("Auto Assign IDs")]
    public void AutoAssignIDs()
    {
        if (items == null)
            return;

#if UNITY_EDITOR
        Undo.RecordObjects(items, "Auto Assign Item IDs");
#endif

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;

            items[i].itemID = i + 1; // 0 = "boş/null" anlamına gelir, 1'den başla

#if UNITY_EDITOR
            EditorUtility.SetDirty(items[i]);
#endif
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif

        RebuildDictionary();
        Debug.Log($"[ItemDatabase] {items.Length} item'a ID atandı. Sıralamayı artık değiştirme!");
    }

    /// <summary>
    /// Mevcut ID'leri ve item isimlerini konsola yazar. Debug için.
    /// </summary>
    [ContextMenu("Print All IDs")]
    public void PrintAllIDs()
    {
        if (items == null)
        {
            Debug.Log("[ItemDatabase] items dizisi boş.");
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                Debug.Log($"  [{i}] NULL");
                continue;
            }

            Debug.Log($"  [{i}] ID: {items[i].itemID} → {items[i].name}");
        }
    }
}