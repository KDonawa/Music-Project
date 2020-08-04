using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : MenuGeneric<LevelCompleteMenu>
{
    [Header("Buttons")]
    [SerializeField] Button homeButton = null;
    [SerializeField] Button restartButton = null;
    [SerializeField] Button nextLevelButton = null;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText = null;

    private void Start()
    {
        if (homeButton == null) Debug.LogError("No reference to Home button");
        else homeButton.onClick.AddListener(OnHomeButtonPressed);

        if (restartButton == null) Debug.LogError("No reference to Restart button");
        else restartButton.onClick.AddListener(OnRestartPressed);

        if (nextLevelButton == null) Debug.LogError("No reference to Next Lvl button");
        else nextLevelButton.onClick.AddListener(OnNextLevelPressed);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeButtonPressed);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
    }
    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsFinalLevel())
        {
            nextLevelButton.gameObject.SetActive(false);
        }
        else nextLevelButton.gameObject.SetActive(true);
    }
    public static void SetFinalScore(float score)
    {
        Instance.scoreText.text = score.ToString() + "%";
    }

    void OnHomeButtonPressed()
    {
        UIAnimator.ButtonPressEffect(homeButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.CIRCLE_WIPE_LEFT, LoadLevelsMenu);
    }
    void OnRestartPressed()
    {
        UIAnimator.ButtonPressEffect(restartButton, AudioManager.click1);
        LevelGameplay gameplay = FindObjectOfType<LevelGameplay>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, gameplay.RestartLevel);
        }
    }
    void OnNextLevelPressed()
    {
        UIAnimator.ButtonPressEffect(nextLevelButton, AudioManager.buttonChime);
        LevelGameplay gameplay = FindObjectOfType<LevelGameplay>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, gameplay.PlayNextLevel);
        }
    }

    
    void LoadLevelsMenu()
    {
        if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();
        GameManager.LoadStartScene();
        LevelsMenu.Open();
    }
    // main menu

    // level select
}
