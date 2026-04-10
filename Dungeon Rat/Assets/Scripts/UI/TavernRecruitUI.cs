using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernRecruitUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TavernManager tavernManager;
    [SerializeField] private TMP_Text tavernLevelText;
    [SerializeField] private GameObject volunteerListRoot;
    [SerializeField] private Button recruitVolunteerButton;
    [SerializeField] private TavernVolunteerButtonUI[] volunteerButtons;

    private void Awake()
    {
        if (tavernManager == null)
            tavernManager = FindFirstObjectByType<TavernManager>();
    }

    private void OnEnable()
    {
        RefreshUI();
        HideVolunteerList();
    }

    public void RefreshUI()
    {
        if (tavernManager == null)
            return;

        if (tavernLevelText != null)
            tavernLevelText.text = $"Tavern Lv. {tavernManager.TavernLevel}";

        if (!tavernManager.CanRecruitVolunteer())
            HideVolunteerList();
    }

    public void OnRecruitVolunteerButtonClicked()
    {
        if (tavernManager == null)
            return;

        RefreshUI();

        if (!tavernManager.CanRecruitVolunteer())
        {
            HideVolunteerList();
            return;
        }

        bool isListVisible = volunteerListRoot != null && volunteerListRoot.activeSelf;

        if (isListVisible)
        {
            HideVolunteerList();
            return;
        }

        ShowVolunteerList();
    }

    private void ShowVolunteerList()
    {
        List<Character> volunteers = tavernManager.GenerateVolunteerChoices();
        BindVolunteerButtons(volunteers);

        if (volunteerListRoot != null)
            volunteerListRoot.SetActive(true);

        if (recruitVolunteerButton != null)
            recruitVolunteerButton.interactable = false;
    }

    private void BindVolunteerButtons(List<Character> volunteers)
    {
        if (volunteerButtons == null)
            return;

        for (int i = 0; i < volunteerButtons.Length; i++)
        {
            bool hasVolunteer = volunteers != null && i < volunteers.Count;

            if (volunteerButtons[i] != null)
                volunteerButtons[i].gameObject.SetActive(hasVolunteer);

            if (!hasVolunteer || volunteerButtons[i] == null)
                continue;

            volunteerButtons[i].Setup(volunteers[i], this);
        }
    }

    public void OnVolunteerSelected(Character volunteer)
    {
        if (tavernManager == null)
            return;

        bool success = tavernManager.RecruitVolunteer(volunteer);
        if (!success)
            return;

        RefreshUI();
        HideVolunteerList();
    }

    public void HideVolunteerList()
    {
        if (volunteerListRoot != null)
            volunteerListRoot.SetActive(false);

        if (recruitVolunteerButton != null)
            recruitVolunteerButton.interactable = true;
    }
}