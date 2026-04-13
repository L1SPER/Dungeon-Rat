using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour,IPointerClickHandler
{
    [Header("Character Features")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classText;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponClassNameText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private TMP_Text slotIndexText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text rangeText;


    [SerializeField] private Image characterImage;

    [Header("Slot Visual")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    [Header("Class Images")]
    [SerializeField] private Sprite warriorSprite;
    [SerializeField] private Sprite archerSprite;
    [SerializeField] private Sprite mageSprite;
    [SerializeField] private Sprite emptySlotSprite;

    private int slotIndex;
    private PartyPreviewUI partyPreviewUI;
    [SerializeField] private CharacterStatsUI characterStatsUI;

    public int GetSlotIndex => slotIndex;
    public void Setup(int index, PartyPreviewUI ui)
    {
        slotIndex = index;
        partyPreviewUI = ui;
        SetSelected(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ConsumableItemContextMenu.Instance?.Hide();
        partyPreviewUI.SelectSlot(slotIndex);
    }
    public void SetSelected(bool isSelected)
    {
        if (backgroundImage!=null)
            backgroundImage.color = isSelected ? selectedColor : normalColor;
    }
    public void SetCharacter(Character character)
    {
        if (character == null)
        {
            SetEmptySlot();
            characterStatsUI.SetEmptyStats();
            return;
        }

        nameText.text = character.name;
        classText.text = character.classType.ToString();
        slotIndexText.text = slotIndex.ToString();

        Weapon equippedWeapon = null;

        if (character.CharacterInventoryObject != null &&
            character.CharacterInventoryObject.inventory != null)
        {
            EquipmentInventory equipmentInventory = character.CharacterInventoryObject.inventory;

            int weaponSlotIndex = equipmentInventory.FindSlotIndexByEquipmentType(EquipmentType.Weapon);

            if (weaponSlotIndex != -1)
            {
                InventorySlot weaponSlot = equipmentInventory.GetSlot(weaponSlotIndex);

                if (weaponSlot != null && !weaponSlot.IsEmpty())
                {
                    equippedWeapon = weaponSlot.item.itemData as Weapon;
                }
            }
        }

        if (equippedWeapon != null)
        {
            weaponNameText.text = equippedWeapon.itemName;
            weaponClassNameText.text = equippedWeapon.WeaponType.ToString();
            weaponDamageText.text = $"{equippedWeapon.minDamage} - {equippedWeapon.maxDamage}";
            rangeText.text = equippedWeapon.range.ToString();
        }
        else
        {
            weaponNameText.text = "Empty";
            weaponClassNameText.text = "-";
            weaponDamageText.text = "-";
            rangeText.text= "1";
        }


        character.position = slotIndex;
        characterImage.sprite = GetClassSprite(character.classType);
        character.characterSprite = characterImage.sprite;
        healthText.text=character.health.currentHealth.ToString()+" / "+ character.health.maxHealth.ToString();

        characterStatsUI.SetStats(character);
    }

    public void SetEmptySlot()
    {
        nameText.text = "Empty Slot";
        classText.text = "-";
        weaponNameText.text = "-";
        weaponClassNameText.text = "-";
        weaponDamageText.text = "-";
        rangeText.text = "-";
        slotIndexText.text = slotIndex.ToString();
        healthText.text = "- / -";

        characterImage.sprite = emptySlotSprite;
    }
    private Sprite GetClassSprite(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Warrior:
                return warriorSprite;

            case ClassType.Archer:
                return archerSprite;

            case ClassType.Mage:
                return mageSprite;

            default:
                return emptySlotSprite;
        }
    }
}