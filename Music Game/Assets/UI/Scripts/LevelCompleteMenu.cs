using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : Menu<LevelCompleteMenu>
{
    [Header("Buttons")]
    [SerializeField] Button homeButton = null;
    [SerializeField] Button restartButton = null;
    [SerializeField] Button nextLevelButton = null;

    [Header("UI")]
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

        Game.LevelCompleteEvent += DisplayMenu;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeButtonPressed);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
    }

    #endregion

    public static void DisplayMenu(int finalScore, bool isLevelPassed)
    {
        // empty text
        Instance.scoreText.text = string.Empty;

        // deactivate stars
        for (int i = 0; i < Instance.stars.Length; i++)
        {
            Instance.stars[i].gameObject.SetActive(false);
        }

        // hide buttons
        Instance.homeButton.gameObject.SetActive(false);
        Instance.restartButton.gameObject.SetActive(false);
        Instance.nextLevelButton.gameObject.SetActive(false);

        MenuManagerUpdated.OpenMenu(Instance);

        Instance.StartCoroutine(Instance.DisplayMenuRoutine(finalScore, isLevelPassed));
    }

    public IEnumerator DisplayMenuRoutine(int score, bool isLevelPassed)
    {
        //yield return new WaitForSeconds(0.5f);

        if (ScoreSystem.Instance != null)
        {
            int finalScore = score;

            // set final score text
            Instance.scoreText.text = finalScore.ToString() + "%";
            //AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
            yield return new WaitForSeconds(1f);

            // display stars earned
            int numStars = 0;
            if (finalScore >= 50) numStars = 1;
            if (isLevelPassed) numStars = 2;
            if (finalScore == 100) numStars = 3;

            int maxNumStars = Instance.stars.Length;
            
            for (int i = 0; i < maxNumStars && i < numStars; i++)
            {
                Instance.stars[i].gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.chime3, SoundType.UI);
                yield return new WaitForSeconds(0.5f);
            }
            if(numStars == 3)
            {
                AudioManager.PlaySound(AudioManager.chime1, SoundType.UI);
                yield return new WaitForSeconds(1f);
            }           

        }
        // display buttons
        Instance.homeButton.gameObject.SetActive(true);
        AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
        yield return new WaitForSeconds(0.2f);

        Instance.restartButton.gameObject.SetActive(true);
        AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
        yield return new WaitForSeconds(0.2f);

        // next level button
        if (isLevelPassed && !GameManager.Instance.IsFinalLevel())
        {
            Instance.nextLevelButton.gameObject.SetActive(isLevelPassed);
            AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
        }        
    }


    #region BUTTON EVENTS

    void OnHomeButtonPressed()
    {
        UIAnimator.ButtonPressEffect3(homeButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.CIRCLE_WIPE_LEFT, LoadLevelsMenu);
    }
    void OnRestartPressed()
    {
        UIAnimator.ButtonPressEffect3(restartButton, AudioManager.click1);

        Game.Stop();

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, Game.Play);
    }
    void OnNextLevelPressed()
    {
        UIAnimator.ButtonPressEffect3(nextLevelButton, AudioManager.buttonChime);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, Game.PlayNextLevel);
    }
    #endregion

    void LoadLevelsMenu()
    {
        GameManager.LoadStartScene();
        LevelSelectMenu.Instance.Open();
    }

}
