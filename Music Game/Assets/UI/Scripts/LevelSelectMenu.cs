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

        levelOptions = new List<Button>();

        if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(MainMenuPressed);

        if (backButton == null) Debug.LogError("No reference to back button");
        else backButton.onClick.AddListener(BackPressed);

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
        if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
    }
    public override void Open()
    {
        LoadData();
        base.Open();
        InitializeButtons();
    }
    void LoadData()
    {
        LevelData data = BinarySaveSystem.LoadLevelData(GameManager.Instance.CurrentStageIndex);
        Level[] levels = GameManager.Instance.GetLevelsInCurrentStage();
        if(levels != null && data != null)
        {
            for (int i = 0; i < levels.Length && i < data.unlockedLevels.Length; i++)
            {
                levels[i].isUnlocked = data.unlockedLevels[i];
            }
        }        
    }
    void InitializeButtons()
    {
        foreach (var b in levelOptions) Destroy(b.gameObject);
        levelOptions.Clear();

        Level[] levels = GameManager.Instance.GetLevelsInCurrentStage();
        if (levels == null) return;
        
        for (int i = 1; i <= GameManager.Instance.GetNumLevelsInCurrentStage(); i++)
        {
            Button b = Instantiate(levelSelectButtonsPrefab, buttonsContainer.transform);
            levelOptions.Add(b);

            if (i == 1) levels[i - 1].isUnlocked = true;
            b.interactable = levels[i - 1].isUnlocked;
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
        UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.OPEN_HORIZONTAL, MainMenu.Instance.Open);
    }
    void BackPressed()
    {
        UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_LEFT, OutTransition.CIRCLE_WIPE_LEFT, StageSelectMenu.Instance.Open);
    }
    
}
