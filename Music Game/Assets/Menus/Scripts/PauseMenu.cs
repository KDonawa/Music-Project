using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MenuTemplpate<PauseMenu>
{
    public void OnResumePressed()
    {
        Time.timeScale = 1f;

        LevelGameplay level = FindObjectOfType<LevelGameplay>(); 
        if (level) level.ResumeGame();
        
        base.OnBackPressed();
        GameMenu.Open();
    }
    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        
        LevelGameplayUtility gameplayUtility = FindObjectOfType<LevelGameplayUtility>();
        if (gameplayUtility != null) gameplayUtility.RestartGame();
    }
    public void OnSettingsPressed()
    {
        SettingsMenu.Open();
    }
    public void OnExitPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene(SceneLoader.START_SCENE_INDEX);
        base.OnBackPressed();
        MainMenu.Open();
        LevelsMenu.Open();
    }

}
