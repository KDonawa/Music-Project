using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DroneSelectMenu : Menu<DroneSelectMenu>
{
    [SerializeField] GameObject noteChoiceContainer = null;
    [SerializeField] GameObject octaveChoiceContainer = null;

    [SerializeField] Button confirmButton = null;
    [SerializeField] Button backButton = null;

    List<Button> noteChoices;
    List<Button> octaveChoices;

    string noteChoice;
    bool isNoteSelected;    
    string octaveChoice;
    bool isOctaveSelected;

    protected override void Awake()
    {
        base.Awake();

        confirmButton.onClick.AddListener(CorfirmButtonPressed);
        backButton.onClick.AddListener(BackButtonPressed);
        InitNoteChoices();
        InitOctaveChoices();
    }

    public override void Open()
    {
        base.Open();
        ResetChoices();
    }

    void ResetChoices()
    {
        confirmButton.gameObject.SetActive(false);
        isNoteSelected = false;
        isOctaveSelected = false;
        noteChoice = string.Empty;
        octaveChoice = string.Empty;
    }    

    void InitNoteChoices()
    {
        noteChoices = new List<Button>();
        foreach (var button in noteChoiceContainer.GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() => NoteSelected(button));
            noteChoices.Add(button);
        }
    }
    void InitOctaveChoices()
    {
        octaveChoices = new List<Button>();
        foreach (var button in octaveChoiceContainer.GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() => OctaveSelected(button));
            octaveChoices.Add(button);
        }
    }
    void CheckIfAllSelected()
    {
        if (isNoteSelected && isOctaveSelected) confirmButton.gameObject.SetActive(true);
    }
    void NoteSelected(Button b)
    {
        UIAnimator.ButtonPressEffect2(b, AudioManager.chime3, Color.white);
        noteChoice = b.GetComponentInChildren<TextMeshProUGUI>().text;
        isNoteSelected = true;

        foreach (var button in noteChoices)
        {
            if (button != b) UIAnimator.SetButtonTextColor(button, Color.black);
        }
        CheckIfAllSelected();
    }
    void OctaveSelected(Button b)
    {
        UIAnimator.ButtonPressEffect2(b, AudioManager.chime3, Color.white);
        octaveChoice = b.GetComponentInChildren<TextMeshProUGUI>().text;
        isOctaveSelected = true;

        foreach (var button in octaveChoices)
        {
            if (button != b) UIAnimator.SetButtonTextColor(button, Color.black);
        }
        CheckIfAllSelected();
    }

    void CorfirmButtonPressed()
    {
        UIAnimator.ButtonPressEffect3(confirmButton, AudioManager.chime3);
        GameManager.Instance.DroneNote = string.Concat(noteChoice, octaveChoice);
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, StageSelectMenu.Instance.Open);
    }

    void BackButtonPressed()
    {
        UIAnimator.ButtonPressEffect3(backButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, MainMenu.Instance.Open);
    }
    
}
