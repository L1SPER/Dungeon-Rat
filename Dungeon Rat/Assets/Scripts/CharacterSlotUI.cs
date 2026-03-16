using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classText;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponClassNameText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private TMP_Text slotIndexText;
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

    public void Setup(int index, PartyPreviewUI ui)
    {
        slotIndex = index;
        partyPreviewUI = ui;
        SetSelected(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //SetSelected(true);
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
            return;
        }

        nameText.text = character.name;
        classText.text = character.classType.ToString();

        if (character.weapon != null)
        {
            weaponNameText.text = character.weapon.weaponName;
            weaponClassNameText.text = character.weapon.weaponType.ToString();
            weaponDamageText.text = character.weapon.minDamage + " - " + character.weapon.maxDamage;
        }
        else
        {
            weaponNameText.text = "No Weapon";
            weaponClassNameText.text = "N/A";
            weaponDamageText.text = "-";
        }

        //KARAKTERIN CANI AYARLANCAK !!!
        slotIndexText.text = slotIndex.ToString();
        character.position = slotIndex;
        characterImage.sprite = GetClassSprite(character.classType);
    }

    public void SetEmptySlot()
    {
        nameText.text = "Empty Slot";
        classText.text = "-";
        weaponNameText.text = "-";
        weaponClassNameText.text = "-";
        weaponDamageText.text = "-";
        slotIndexText.text = slotIndex.ToString();
        //KARAKTERIN CANI AYARLANCAK !!!

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