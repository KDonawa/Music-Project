using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelsMenu : MenuGeneric<LevelsMenu>
{
    [Header("Buttons")]    
    [SerializeField] Button mainMenuButton = null;
    [SerializeField] Button stageMenuButton = null;
    [SerializeField] GameObject buttonsContainer = null;

    [Header("Prefabs")]
    [SerializeField] Button levelSelectButtonsPrefab = null;

    List<Button> levelOptions;

    private void Start()
    {
        if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(OnMainMenuPressed);

        if (stageMenuButton == null) Debug.LogError("No reference to Stage Menu button");
        else stageMenuButton.onClick.AddListener(OnStageMenuPressed);

        InitializeButtons();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenuPressed);
        if (stageMenuButton != null) stageMenuButton.onClick.RemoveListener(OnMainMenuPressed);
    }

    void InitializeButtons()
    {
        levelOptions = new List<Button>();

        int numLevels = GameManager.Instance.GetNumLevelsInCurrentStage();
        for (int i = 1; i <= numLevels; i++)
        {
            Button b = Instantiate(levelSelectButtonsPrefab, buttonsContainer.transform);
            levelOptions.Add(b);
            b.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            LevelSelectButton lsb = b.GetComponent<LevelSelectButton>();
            lsb.InitializeButton(i);
            b.onClick.AddListener(() => lsb.ButtonPressed(ButtonPressedEffect));
        }
    }
    void ButtonPressedEffect(Button button)
    {
        buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
        foreach (var b in levelOptions)
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
        buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;
        rect.gameObject.SetActive(true);
    }
    void OnMainMenuPressed()
    {
        UIAnimator.ButtonPressEffect(mainMenuButton, AudioManager.click1);
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.OPEN_HORIZONTAL, MainMenu.Open);
    }
    void OnStageMenuPressed()
    {
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_LEFT, OutTransition.CIRCLE_WIPE_LEFT, StageSelectMenu.Open);
    }
    
}
