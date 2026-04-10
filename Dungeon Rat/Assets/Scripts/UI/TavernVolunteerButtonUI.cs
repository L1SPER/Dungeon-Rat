using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernVolunteerButtonUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text classText;

    [Header("Class Sprites")]
    [SerializeField] private Sprite warriorSprite;
    [SerializeField] private Sprite archerSprite;
    [SerializeField] private Sprite mageSprite;

    private Character currentCharacter;
    private TavernRecruitUI tavernRecruitUI;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Setup(Character character, TavernRecruitUI recruitUI)
    {
        currentCharacter = character;
        tavernRecruitUI = recruitUI;

        if (nameText != null)
            nameText.text = character != null ? character.name : "-";

        if (classText != null)
            classText.text = character != null ? character.classType.ToString() : "-";

        if (characterImage != null && character != null)
        {
            characterImage.sprite = GetClassSprite(character.classType);
            characterImage.enabled = characterImage.sprite != null;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        if (currentCharacter == null || tavernRecruitUI == null)
            return;

        tavernRecruitUI.OnVolunteerSelected(currentCharacter);
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
                return null;
        }
    }
}