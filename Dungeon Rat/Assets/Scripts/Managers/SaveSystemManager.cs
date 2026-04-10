using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystemManager : MonoBehaviour
{
    public static SaveSystemManager Instance;

    [Header("References")]
    [SerializeField] private InventorySaveRegistry inventoryRegistry;
    [SerializeField] private DungeonManager dungeonManager;
    [SerializeField] private PartyManager partyManager;

    [Header("Settings")]
    [SerializeField] private string fileName = "savegame.json";
    [SerializeField] private bool loadOnAwake = true;
    [SerializeField] private bool saveOnApplicationPause = true;
    [SerializeField] private bool saveOnApplicationQuit = true;
    [SerializeField] private bool saveEnabled = true;
    [SerializeField] private bool loadEnabled = true;

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, fileName);

    private GameSaveData cachedLoadedSaveData;

    public bool HasLoadedSave { get; private set; }
    public bool HasSaveFile => File.Exists(SaveFilePath);
    public bool IsSaveEnabled => saveEnabled;
    public bool IsLoadEnabled => loadEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (loadOnAwake)
            LoadGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && saveOnApplicationPause)
            SaveGame();
    }

    private void OnApplicationQuit()
    {
        if (saveOnApplicationQuit)
            SaveGame();
    }

    public void SaveGame()
    {
        if (!saveEnabled)
        {
            Debug.Log("SaveSystemManager: Save kapalı, kayıt alınmadı.");
            return;
        }

        if (inventoryRegistry == null)
        {
            Debug.LogWarning("SaveSystemManager: InventorySaveRegistry referansı eksik.");
            return;
        }

        GameSaveData saveData = BuildSaveData();
        string json = JsonUtility.ToJson(saveData, true);

        string directory = Path.GetDirectoryName(SaveFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"Game saved: {SaveFilePath}");
    }

    public bool LoadGame()
    {
        if (!loadEnabled)
        {
            Debug.Log("SaveSystemManager: Load kapalı, kayıt yüklenmedi.");
            return false;
        }

        if (!HasSaveFile)
        {
            Debug.Log("SaveSystemManager: Kayıt dosyası bulunamadı.");
            return false;
        }

        if (inventoryRegistry == null)
        {
            Debug.LogWarning("SaveSystemManager: InventorySaveRegistry referansı eksik.");
            return false;
        }

        string json = File.ReadAllText(SaveFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return false;

        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        if (saveData == null)
            return false;

        cachedLoadedSaveData = saveData;
        ApplySaveData(saveData);
        HasLoadedSave = true;

        Debug.Log($"Game loaded: {SaveFilePath}");
        return true;
    }

    public bool TryApplyLoadedParty()
    {
        if (cachedLoadedSaveData == null || cachedLoadedSaveData.party == null)
            return false;

        PartyManager resolvedPartyManager = GetPartyManager();
        if (resolvedPartyManager == null)
            return false;

        return resolvedPartyManager.TryLoadParty(cachedLoadedSaveData.party);
    }

    public void DeleteSave()
    {
        if (!HasSaveFile)
            return;

        File.Delete(SaveFilePath);
        HasLoadedSave = false;
        cachedLoadedSaveData = null;
        Debug.Log("Save file deleted.");
    }

    private PartyManager GetPartyManager()
    {
        if (partyManager == null)
            partyManager = FindFirstObjectByType<PartyManager>();

        return partyManager;
    }

    private GameSaveData BuildSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.lastSceneName = SceneManager.GetActiveScene().name;
        saveData.savedAtUtc = DateTime.UtcNow.ToString("O");

        foreach (InventoryObjectReference inventoryRef in inventoryRegistry.GetAllInventories())
        {
            InventorySaveData inventorySave = CreateInventorySaveData(inventoryRef);
            if (inventorySave != null)
                saveData.inventories.Add(inventorySave);
        }

        if (dungeonManager != null)
        {
            saveData.dungeonProgression = dungeonManager.GetProgressionSaveData();
            saveData.activeDungeonRun = dungeonManager.GetCurrentRunSaveData();
        }

        PartyManager resolvedPartyManager = GetPartyManager();
        if (resolvedPartyManager != null)
            saveData.party = resolvedPartyManager.CreatePartySaveData();

        return saveData;
    }

    private void ApplySaveData(GameSaveData saveData)
    {
        Dictionary<string, InventorySaveData> inventoryMap = new Dictionary<string, InventorySaveData>();

        if (saveData.inventories != null)
        {
            for (int i = 0; i < saveData.inventories.Count; i++)
            {
                InventorySaveData inventorySave = saveData.inventories[i];
                if (inventorySave == null || string.IsNullOrWhiteSpace(inventorySave.inventoryKey))
                    continue;

                inventoryMap[inventorySave.inventoryKey] = inventorySave;
            }
        }

        foreach (InventoryObjectReference inventoryRef in inventoryRegistry.GetAllInventories())
        {
            if (!inventoryMap.TryGetValue(inventoryRef.Key, out InventorySaveData inventorySave))
                continue;

            ApplyInventorySaveData(inventoryRef, inventorySave);
        }

        if (dungeonManager != null)
        {
            dungeonManager.ApplyProgressionSaveData(saveData.dungeonProgression);
            dungeonManager.ApplyCurrentRunSaveData(saveData.activeDungeonRun);
        }

        PartyManager resolvedPartyManager = GetPartyManager();
        if (resolvedPartyManager != null && saveData.party != null)
            resolvedPartyManager.TryLoadParty(saveData.party);
    }

    private InventorySaveData CreateInventorySaveData(InventoryObjectReference inventoryRef)
    {
        if (inventoryRef.Inventory == null)
            return null;

        InventorySaveData saveData = new InventorySaveData();
        saveData.inventoryKey = inventoryRef.Key;

        if (inventoryRef.Inventory is GeneralInventory generalInventory)
        {
            saveData.level = generalInventory.Level;
            saveData.currentCapacity = generalInventory.CurrentCapacity;
            saveData.maxCapacity = generalInventory.MaxCapacity;
            saveData.useOverflowSlots = generalInventory.UseOverflowSlots;
        }
        else if (inventoryRef.Inventory is RatInventory ratInventory)
        {
            saveData.level = ratInventory.Level;
            saveData.currentCapacity = ratInventory.CurrentCapacity;
            saveData.maxCapacity = ratInventory.MaxCapacity;
            saveData.useOverflowSlots = ratInventory.UseOverflowSlots;
        }
        else if (inventoryRef.Inventory is LootInventory lootInventory)
        {
            saveData.level = 0;
            saveData.currentCapacity = lootInventory.CurrentCapacity;
            saveData.maxCapacity = lootInventory.MaxCapacity;
            saveData.useOverflowSlots = lootInventory.UseOverflowSlots;
        }
        else if (inventoryRef.Inventory is EquipmentInventory)
        {
            saveData.level = 0;
        }

        InventorySlot[] slots = inventoryRef.Inventory.Slots;
        if (slots == null)
            return saveData;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i];
            if (slot == null || slot.IsEmpty())
                continue;

            if (slot.item == null || slot.item.itemData == null)
                continue;

            InventorySlotSaveData slotSave = new InventorySlotSaveData();
            slotSave.slotIndex = i;
            slotSave.itemId = slot.item.itemData.itemID;
            slotSave.amount = slot.item.amount;
            slotSave.isOverflowSlot = slot.isOverflowSlot;
            slotSave.allowedEquipmentType = (int)slot.AllowedEquipmentType;
            saveData.slots.Add(slotSave);
        }

        return saveData;
    }

    private void ApplyInventorySaveData(InventoryObjectReference inventoryRef, InventorySaveData saveData)
    {
        if (inventoryRef.Inventory == null || inventoryRef.ItemDatabase == null || saveData == null)
            return;

        if (inventoryRef.Inventory is GeneralInventory generalInventory)
        {
            generalInventory.ConfigureByLevel(Mathf.Max(1, saveData.level));
        }
        else if (inventoryRef.Inventory is RatInventory ratInventory)
        {
            ratInventory.ConfigureByLevel(Mathf.Max(1, saveData.level));
        }
        else if (inventoryRef.Inventory is LootInventory lootInventory)
        {
            lootInventory.Configure(saveData.currentCapacity, saveData.maxCapacity, saveData.useOverflowSlots);
        }
        else if (inventoryRef.Inventory is EquipmentInventory equipmentInventory)
        {
            equipmentInventory.ConfigureDefaultRestrictions();
        }

        inventoryRef.Inventory.Clear();
        InventorySlot[] slots = inventoryRef.Inventory.Slots;
        if (slots == null || saveData.slots == null)
            return;

        for (int i = 0; i < saveData.slots.Count; i++)
        {
            InventorySlotSaveData slotSave = saveData.slots[i];
            if (slotSave == null)
                continue;

            if (slotSave.slotIndex < 0 || slotSave.slotIndex >= slots.Length)
                continue;

            ItemData itemData = inventoryRef.ItemDatabase.GetItemByID(slotSave.itemId);
            if (itemData == null)
            {
                Debug.LogWarning($"Item ID çözülemedi: {slotSave.itemId} | Inventory: {inventoryRef.Key}");
                continue;
            }

            InventorySlot slot = slots[slotSave.slotIndex];
            if (slot == null)
                continue;

            slot.SetAllowedEquipmentType((EquipmentType)slotSave.allowedEquipmentType);
            slot.SetItem(itemData, Mathf.Max(1, slotSave.amount));
        }

        inventoryRef.Inventory.NotifyInventoryChanged();
    }

    public void SetSaveEnabled(bool isEnabled)
    {
        saveEnabled = isEnabled;
        Debug.Log($"SaveSystemManager: Save sistemi {(saveEnabled ? "aktif" : "deaktif")}.");
    }

    public void EnableSave()
    {
        SetSaveEnabled(true);
    }

    public void DisableSave()
    {
        SetSaveEnabled(false);
    }

    public void ToggleSaveEnabled()
    {
        SetSaveEnabled(!saveEnabled);
    }

    public void SetLoadEnabled(bool isEnabled)
    {
        loadEnabled = isEnabled;
        Debug.Log($"SaveSystemManager: Load sistemi {(loadEnabled ? "aktif" : "deaktif")}.");
    }

    public void EnableLoad()
    {
        SetLoadEnabled(true);
    }

    public void DisableLoad()
    {
        SetLoadEnabled(false);
    }

    public void ToggleLoadEnabled()
    {
        SetLoadEnabled(!loadEnabled);
    }

    [ContextMenu("Enable Save")]
    private void ContextEnableSave()
    {
        EnableSave();
    }

    [ContextMenu("Disable Save")]
    private void ContextDisableSave()
    {
        DisableSave();
    }

    [ContextMenu("Toggle Save")]
    private void ContextToggleSave()
    {
        ToggleSaveEnabled();
    }

    [ContextMenu("Enable Load")]
    private void ContextEnableLoad()
    {
        EnableLoad();
    }

    [ContextMenu("Disable Load")]
    private void ContextDisableLoad()
    {
        DisableLoad();
    }

    [ContextMenu("Toggle Load")]
    private void ContextToggleLoad()
    {
        ToggleLoadEnabled();
    }
}