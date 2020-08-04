using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectMenu : MenuGeneric<StageSelectMenu>
{
    [Header("References")]
    [SerializeField] GameObject buttonsContainer = null;
    [SerializeField] Button mainMenuButton = null;

    [Header("UI/Prefabs")]
    [SerializeField] Button stageSelectButtonsPrefab = null;

    List<Button> stageOptions;

    private void Start()
    {
        if(mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(OnMainMenuPressed);
        
        InitializeButtons();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenuPressed);
    }

    void InitializeButtons()
    {
        stageOptions = new List<Button>();

        Stage[] stages = GameManager.Instance.GetStages();
        for (int i = 1; i <= GameManager.Instance.NumStages; i++)
        {
            Button b = Instantiate(stageSelectButtonsPrefab, buttonsContainer.transform);
            stageOptions.Add(b);
            b.interactable = stages[i-1].isUnlocked;
            b.GetComponentInChildren<TextMeshProUGUI>().text = stages[i-1].name.ToString();

            StageSelectButton ssb = b.GetComponent<StageSelectButton>();
            ssb.InitializeButton(i);
            //b.onClick.AddListener(delegate { ssb.ButtonPressed(OnButtonPressed); });
            b.onClick.AddListener(() => ssb.ButtonPressed(ButtonPressedEffect));
        }
    }
    
    void ButtonPressedEffect(Button button)
    {
        buttonsContainer.GetComponent<VerticalLayoutGroup>().enabled = false;
        foreach (var b in stageOptions)
        {
            if (b != button)
            {
                RectTransform rect = b.GetComponent<RectTransform>();
                UIAnimator.ShrinkToNothing(rect, 0.5f, 2f, () => ButtonPressEffectCompleted(rect));
            }
        }
    }
    void ButtonPressEffectCompleted(RectTransform rect)
    {
        buttonsContainer.GetComponent<VerticalLayoutGroup>().enabled = true;
        rect.gameObject.SetActive(true);
    }

    void OnMainMenuPressed()
    {
        UIAnimator.ButtonPressEffect(mainMenuButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, MainMenu.Open);
    }

}
