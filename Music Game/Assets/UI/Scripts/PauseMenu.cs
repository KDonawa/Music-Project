using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseMenu : MenuGeneric<PauseMenu>
{
    [Header("Buttons")]
    [SerializeField] Button resumeButton = null;
    [SerializeField] Button restartButton = null;
    [SerializeField] Button settingsButton = null;
    [SerializeField] Button exitButton = null;

    private void Start()
    {
        if (resumeButton == null) Debug.LogError("No reference to resume button");
        else resumeButton.onClick.AddListener(OnResumePressed);

        if (restartButton == null) Debug.LogError("No reference to restart button");
        else restartButton.onClick.AddListener(OnRestartPressed);

        if (settingsButton == null) Debug.LogError("No reference to settings button");
        else settingsButton.onClick.AddListener(OnSettingsPressed);

        if (exitButton == null) Debug.LogError("No reference to exit button");
        else exitButton.onClick.AddListener(OnExitPressed);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumePressed);
        if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsPressed);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitPressed);
    }
    void OnResumePressed()
    {
        Time.timeScale = 1f;

        UIAnimator.ButtonPressEffect(resumeButton, AudioManager.click1);

        Game game = FindObjectOfType<Game>();
        if (game) game.ResumeGame();
        
        base.OnBackPressed();
        GameMenu.Open();
    }
    void OnRestartPressed()
    {
        Time.timeScale = 1f;

        UIAnimator.ButtonPressEffect(restartButton, AudioManager.buttonChime);

        Game game = FindObjectOfType<Game>();
        if (game != null)
        {          
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, game.RestartLevel);
        }
    }
    void OnSettingsPressed()
    {
        UIAnimator.ButtonPressEffect(settingsButton, AudioManager.click1);
        //SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, SettingsMenu.Open);
        SettingsMenu.Open();
    }
    void OnExitPressed()
    {
        Time.timeScale = 1f;
        UIAnimator.ButtonPressEffect(exitButton, AudioManager.click1);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_UP, LoadLevelsMenu);
        
    }
    void LoadLevelsMenu()
    {
        GameManager.LoadStartScene();
        LevelsMenu.Open();        
    }


}
