using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classText;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponClassNameText;
    [SerializeField] private TMP_Text weaponDamageText;
    [SerializeField] private Image characterImage;

    [Header("Class Images")]
    [SerializeField] private Sprite warriorSprite;
    [SerializeField] private Sprite archerSprite;
    [SerializeField] private Sprite mageSprite;
    [SerializeField] private Sprite emptySlotSprite;

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

        characterImage.sprite = GetClassSprite(character.classType);
    }

    public void SetEmptySlot()
    {
        nameText.text = "Empty Slot";
        classText.text = "-";
        weaponNameText.text = "-";
        weaponClassNameText.text = "-";
        weaponDamageText.text = "-";

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