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
    [SerializeField] GameObject starsContainer = null;

    Image[] stars;
    protected override void Awake()
    {
        base.Awake();
        if(starsContainer != null) stars = starsContainer.GetComponentsInChildren<Image>();

        //Game.LevelCompleteEvent += DisplayMenu;
    }


    #region SETUP
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

        //Game.LevelCompleteEvent -= DisplayMenu;
    }

    #endregion

    public static void DisplayMenu(bool isLevelPassed)
    {
        //Debug.Log("test");
        if (ScoreSystem.Instance != null)
        {
            int finalScore = ScoreSystem.GetPlayerScorePercentageAsInt();

            // set final score text
            Instance.scoreText.text = finalScore.ToString() + "%";

            // display stars earned
            int numStars = 0;
            if (finalScore >= 50) numStars = 1;
            if (isLevelPassed) numStars = 2;
            if (finalScore == 100) numStars = 3;

            int maxNumStars = Instance.stars.Length;
            for (int i = 0; i < maxNumStars; i++)
            {
                Instance.stars[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < maxNumStars && i < numStars; i++)
            {
                Instance.stars[i].gameObject.SetActive(true);
            }            
        }
        // next level button
        Instance.nextLevelButton.gameObject.SetActive(isLevelPassed);
        if (GameManager.Instance != null && GameManager.Instance.IsFinalLevel())
        {
            Instance.nextLevelButton.gameObject.SetActive(false);
        }
        
        MenuManager.Instance.OpenMenu(Instance);
    }


    #region BUTTON EVENTS

    void OnHomeButtonPressed()
    {
        UIAnimator.ButtonPressEffect(homeButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.CIRCLE_WIPE_LEFT, LoadLevelsMenu);
    }
    void OnRestartPressed()
    {
        UIAnimator.ButtonPressEffect(restartButton, AudioManager.click1);
        Game gameplay = FindObjectOfType<Game>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, gameplay.RestartLevel);
        }
    }
    void OnNextLevelPressed()
    {
        UIAnimator.ButtonPressEffect(nextLevelButton, AudioManager.buttonChime);
        Game gameplay = FindObjectOfType<Game>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, gameplay.PlayNextLevel);
        }
    }
    #endregion

    void LoadLevelsMenu()
    {
        if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();
        GameManager.LoadStartScene();
        LevelsMenu.Open();
    }
    // main menu

    // level select
}
