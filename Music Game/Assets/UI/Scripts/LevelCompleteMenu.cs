using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : Menu<LevelCompleteMenu>
{
    [Header("Buttons")]
    //[SerializeField] Button mainMenuButton = null;
    [SerializeField] Button homeButton = null;
    [SerializeField] Button restartButton = null;
    [SerializeField] Button nextLevelButton = null;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI headerText = null;
    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] GameObject starsContainer = null;

    Image[] stars;


    #region SETUP
    protected override void Awake()
    {
        base.Awake();
        if (starsContainer != null) stars = starsContainer.GetComponentsInChildren<Image>();

        if (homeButton == null) Debug.LogError("No reference to Home button");
        else homeButton.onClick.AddListener(OnHomeButtonPressed);

        if (restartButton == null) Debug.LogError("No reference to Restart button");
        else restartButton.onClick.AddListener(OnRestartPressed);

        if (nextLevelButton == null) Debug.LogError("No reference to Next Lvl button");
        else nextLevelButton.onClick.AddListener(OnNextLevelPressed);

        //mainMenuButton.onClick.AddListener(MainMenuPressed);

        Game.LevelCompleteEvent += DisplayMenu;
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeButtonPressed);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
        //mainMenuButton.onClick.RemoveListener(MainMenuPressed);

        Game.LevelCompleteEvent -= DisplayMenu;
    }

    #endregion

    public static void DisplayMenu(int finalScore, bool isLevelPassed)
    {
        Instance.headerText.text = isLevelPassed ? "Level Complete!" : "Try Again";

        Instance.scoreText.gameObject.SetActive(false);

        // deactivate stars
        for (int i = 0; i < Instance.stars.Length; i++)
        {
            Instance.stars[i].gameObject.SetActive(false);
        }

        // hide buttons
        Instance.homeButton.gameObject.SetActive(false);
        Instance.restartButton.gameObject.SetActive(false);
        Instance.nextLevelButton.gameObject.SetActive(false);

        Instance.Open();

        Instance.StartCoroutine(Instance.DisplayMenuRoutine(finalScore, isLevelPassed));
    }

    public IEnumerator DisplayMenuRoutine(int score, bool isLevelPassed)
    {
        yield return new WaitForSeconds(0.5f);

        if (ScoreSystem.Instance != null)
        {
            int finalScore = score;

            // set final score text
            // make this into a routine where it increases score by one point until final score
            Instance.scoreText.text = finalScore.ToString() + "%";
            Instance.scoreText.gameObject.SetActive(true);
            //AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
            yield return new WaitForSeconds(1f);

            // display stars earned
            int numStars = 0;
            if (finalScore >= 50) numStars = 1;
            if (isLevelPassed) numStars = 2;
            if (finalScore == 100) numStars = 3;

            // earned stars in lvl data
            Level currentLevel = GameManager.Instance.GetCurrentLevel();
            if (numStars > currentLevel.numStarsEarned) currentLevel.numStarsEarned = numStars;
            BinarySaveSystem.SaveLevelData(); // temporary

            // display stars
            numStars = Mathf.Clamp(numStars, 0, Instance.stars.Length);
            for (int i = 0; i < numStars && i < numStars; i++)
            {
                Instance.stars[i].gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.starDisplay, SoundType.UI);
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return new WaitForSeconds(1f);

        // open stage complete menu
        if (isLevelPassed && GameManager.Instance.IsFinalLevel() /*&& !GameManager.Instance.IsCurrentStageCompleted()*/)
        {
            GameManager.Instance.UnlockNextStage();
            yield return new WaitForSeconds(1f);
            StageCompleteMenu.Instance.Open();           
        }
        // display buttons
        else
        {
            homeButton.gameObject.SetActive(true);
            homeButton.interactable = false;
            AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.UI);
            yield return new WaitForSeconds(0.2f);

            restartButton.gameObject.SetActive(true);
            restartButton.interactable = false;
            AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.UI);
            yield return new WaitForSeconds(0.2f);

            if (isLevelPassed && !GameManager.Instance.IsFinalLevel())
            {
                GameManager.Instance.UnlockNextLevel();
                nextLevelButton.gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.UI);
            }

            homeButton.interactable = true;
            restartButton.interactable = true;
        }
    }


    #region BUTTON EVENTS
    //void MainMenuPressed()
    //{
    //    UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
    //    SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, MainMenu.Instance.Open);
    //}

    void OnHomeButtonPressed()
    {
        UIAnimator.ButtonPressEffect3(homeButton, AudioManager.buttonSelect2);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.CIRCLE_WIPE_LEFT, 
            () => GameManager.LoadStartScene(LevelSelectMenu.Instance.Open));
    }
    void OnRestartPressed()
    {
        UIAnimator.ButtonPressEffect3(restartButton, AudioManager.buttonSelect2);

        Game.Stop();

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, Game.Play);
    }
    void OnNextLevelPressed()
    {
        UIAnimator.ButtonPressEffect3(nextLevelButton, AudioManager.buttonSelect1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, Game.PlayNextLevel);
    }
    #endregion

}
