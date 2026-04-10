using System.Collections.Generic;
using UnityEngine;

public class TavernManager : MonoBehaviour
{
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private ItemDatabaseObject itemDatabase;
    [SerializeField, Range(1, 5)] private int tavernLevel = 1;

    private readonly List<Character> currentVolunteers = new List<Character>();

    public int TavernLevel => tavernLevel;
    public IReadOnlyList<Character> CurrentVolunteers => currentVolunteers;

    private void Awake()
    {
        if (partyManager == null)
            partyManager = FindFirstObjectByType<PartyManager>();
    }

    public bool CanRecruitVolunteer()
    {
        return partyManager != null && partyManager.GetAlivePartySize() < 3;
    }

    public void UpgradeTavern()
    {
        tavernLevel = Mathf.Clamp(tavernLevel + 1, 1, 5);
        currentVolunteers.Clear();
    }

    public void SetTavernLevel(int level)
    {
        tavernLevel = Mathf.Clamp(level, 1, 5);
        currentVolunteers.Clear();
    }

    public List<Character> GenerateVolunteerChoices()
    {
        if (currentVolunteers.Count == 0)
        {
            for (int i = 0; i < tavernLevel; i++)
                currentVolunteers.Add(CreateVolunteer());
        }

        return new List<Character>(currentVolunteers);
    }

    public bool RecruitVolunteer(Character selectedVolunteer)
    {
        if (selectedVolunteer == null)
        {
            Debug.LogWarning("Selected volunteer is null.");
            return false;
        }

        if (!CanRecruitVolunteer())
        {
            Debug.Log("Recruit Volunteer butonu pasif olmalıydı. Parti zaten dolu.");
            return false;
        }

        EquipmentItemData tavernWeapon = GetWeaponByTavernLevel();
        bool success = partyManager != null && partyManager.RecruitCharacter(selectedVolunteer, tavernWeapon);

        if (success)
            currentVolunteers.Clear();

        return success;
    }

    private Character CreateVolunteer()
    {
        ClassType randomClass = GetRandomClass();
        string randomName = GetRandomName(randomClass);

        return new Character(randomName, randomClass);
    }

    public ClassType GetRandomClass()
    {
        int count = System.Enum.GetValues(typeof(ClassType)).Length;
        return (ClassType)Random.Range(0, count);
    }

    public EquipmentItemData GetWeaponByTavernLevel()
    {
        ItemRarity targetRarity = GetRarityByTavernLevel();

        EquipmentItemData weapon = GetRandomWeaponByRarity(targetRarity);

        if (weapon != null)
            return weapon;

        Debug.LogWarning($"Tavern Level {tavernLevel} için {targetRarity} rarity weapon bulunamadı. Fallback weapon veriliyor.");

        return GetAnyRandomWeapon();
    }

    private ItemRarity GetRarityByTavernLevel()
    {
        switch (tavernLevel)
        {
            case 1:
                return ItemRarity.Common;
            case 2:
                return ItemRarity.Uncommon;
            case 3:
                return ItemRarity.Rare;
            case 4:
                return ItemRarity.Epic;
            case 5:
                return ItemRarity.Legendary;
            default:
                return ItemRarity.Common;
        }
    }

    private EquipmentItemData GetRandomWeaponByRarity(ItemRarity rarity)
    {
        if (itemDatabase == null || itemDatabase.items == null || itemDatabase.items.Length == 0)
        {
            Debug.LogWarning("Item database eksik. Weapon üretilemedi.");
            return null;
        }

        List<EquipmentItemData> possibleWeapons = new List<EquipmentItemData>();

        for (int i = 0; i < itemDatabase.items.Length; i++)
        {
            EquipmentItemData equipmentItem = itemDatabase.items[i] as EquipmentItemData;
            if (equipmentItem == null)
                continue;

            if (equipmentItem.equipmentType != EquipmentType.Weapon)
                continue;

            if (equipmentItem.itemRarity != rarity)
                continue;

            possibleWeapons.Add(equipmentItem);
        }

        if (possibleWeapons.Count == 0)
            return null;

        return possibleWeapons[Random.Range(0, possibleWeapons.Count)];
    }

    private EquipmentItemData GetAnyRandomWeapon()
    {
        if (itemDatabase == null || itemDatabase.items == null || itemDatabase.items.Length == 0)
        {
            Debug.LogWarning("Item database eksik. Random weapon üretilemedi.");
            return null;
        }

        List<EquipmentItemData> possibleWeapons = new List<EquipmentItemData>();

        for (int i = 0; i < itemDatabase.items.Length; i++)
        {
            EquipmentItemData equipmentItem = itemDatabase.items[i] as EquipmentItemData;
            if (equipmentItem == null)
                continue;

            if (equipmentItem.equipmentType != EquipmentType.Weapon)
                continue;

            possibleWeapons.Add(equipmentItem);
        }

        if (possibleWeapons.Count == 0)
        {
            Debug.LogWarning("Database içinde weapon bulunamadı.");
            return null;
        }

        return possibleWeapons[Random.Range(0, possibleWeapons.Count)];
    }

    private string GetRandomName(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Warrior:
                string[] warriors =
                {
                    "Borin", "Kael", "Durgan", "Thorin", "Ragnar",
                    "Garrik", "Ulric", "Magnus", "Hector", "Brakus",
                    "Valen", "Drogan", "Tarkon", "Roderik", "Baldric"
                };
                return warriors[Random.Range(0, warriors.Length)];

            case ClassType.Archer:
                string[] archers =
                {
                    "Lira", "Sera", "Nessa", "Elira", "Faelyn",
                    "Sylra", "Kara", "Nyra", "Aeris", "Selene",
                    "Talia", "Vera", "Rina", "Lyra", "Arwen"
                };
                return archers[Random.Range(0, archers.Length)];

            case ClassType.Mage:
                string[] mages =
                {
                    "Eldrin", "Mira", "Vael", "Altheron", "Zerath",
                    "Lyrius", "Velora", "Orin", "Seraphis", "Malver",
                    "Ilyra", "Azriel", "Thalor", "Varyn", "Celindra"
                };
                return mages[Random.Range(0, mages.Length)];
        }

        return "Volunteer";
    }
}