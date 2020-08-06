using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectMenu : Menu<LevelSelectMenu>
{
    [Header("Buttons")]    
    [SerializeField] Button mainMenuButton = null;
    [SerializeField] Button backButton = null;
    [SerializeField] GameObject buttonsContainer = null;

    [Header("Prefabs")]
    [SerializeField] Button levelSelectButtonsPrefab = null;

    List<Button> levelOptions;

    protected override void Awake()
    {
        base.Awake();

        if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(MainMenuPressed);

        if (backButton == null) Debug.LogError("No reference to back button");
        else backButton.onClick.AddListener(BackPressed);

        InitializeButtons();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
        if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
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
    void MainMenuPressed()
    {
        UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.click1);
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.OPEN_HORIZONTAL, MainMenu.Instance.Open);
    }
    void BackPressed()
    {
        UIAnimator.ButtonPressEffect3(backButton, AudioManager.click1);
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_LEFT, OutTransition.CIRCLE_WIPE_LEFT, StageSelectMenu.Instance.Open);
    }
    
}
