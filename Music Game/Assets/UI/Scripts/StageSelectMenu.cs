using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectMenu : Menu<StageSelectMenu>
{
    [Header("References")]
    [SerializeField] GameObject buttonsContainer = null;
    [SerializeField] Button backButton = null;
    [SerializeField] Button mainMenuButton = null;

    [Header("UI/Prefabs")]
    [SerializeField] Button stageSelectButtonsPrefab = null;

    List<Button> stageOptions;

    protected override void Awake()
    {
        base.Awake();

        if (backButton == null) Debug.LogError("No reference to back button");
        else backButton.onClick.AddListener(BackPressed);

        if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(MainMenuPressed);

        InitializeButtons();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
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

    void BackPressed()
    {
        UIAnimator.ButtonPressEffect3(backButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, DroneSelectMenu.Instance.Open);
    }
    void MainMenuPressed()
    {
        UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.click1);
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, MainMenu.Instance.Open);
    }
}
