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
    [SerializeField] Button unlockedButtonPrefab = null;
    [SerializeField] Button lockedButtonPrefab = null;
    

    List<Button> stageOptions;

    protected override void Awake()
    {
        base.Awake();

        stageOptions = new List<Button>();

        if (backButton == null) Debug.LogError("No reference to back button");
        else backButton.onClick.AddListener(BackPressed);

        if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
        else mainMenuButton.onClick.AddListener(MainMenuPressed);

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
    }
    public override void Open()
    {       
        InitializeMenu();
        base.Open();
    }
    void LoadData()
    {
        StageSaveData data = BinarySaveSystem.LoadStageData();
        Stage[] stages = GameManager.Stages;
        if (stages != null && data != null)
        {
            for (int i = 0; i < GameManager.NumStages && i < data.unlockedStages.Length; i++)
            {
                if (data != null && stages[i] != null)
                {
                    stages[i].isUnlocked = data.unlockedStages[i];
                    stages[i].numPassedLevels = data.numPassedLevels[i];
                }
            }
        }        
    }
    void InitializeMenu()
    {
        LoadData();

        foreach (var b in stageOptions) Destroy(b.gameObject);
        stageOptions.Clear();        

        Stage[] stages = GameManager.Stages;
        for (int i = 1; i <= GameManager.NumStages; i++)
        {
            Button b;           
                      
            if (i == 1) stages[i - 1].isUnlocked = true;
            if(stages[i - 1].isUnlocked) b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);
            else b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
            stageOptions.Add(b);
            b.interactable = stages[i - 1].isUnlocked;
            //b.GetComponentInChildren<TextMeshProUGUI>().text = i + ". " + stages[i-1].name;
            StageSelectButton ssb = b.GetComponent<StageSelectButton>();
            ssb.Init(i, stages[i - 1].name, stages[i-1].numPassedLevels);
            b.onClick.AddListener(() => ssb.ButtonPressed(ButtonPressedEffect));
        }
    }
    
    void ButtonPressedEffect(Button button)
    {
        buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
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
        buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;
        rect.gameObject.SetActive(true);
    }

    void BackPressed()
    {
        UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, DroneSelectMenu.Instance.Open);
    }
    void MainMenuPressed()
    {
        UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
    }
}
