using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MenuGeneric<PauseMenu>
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
        
        LevelGameplay gameplay = FindObjectOfType<LevelGameplay>();
        if (gameplay != null) gameplay.RestartLevel();
    }
    public void OnSettingsPressed()
    {
        SettingsMenu.Open();
    }
    public void OnExitPressed()
    {
        Time.timeScale = 1f;
        GameManager.LoadScene("MenuScene");
        //base.OnBackPressed();
        //MainMenu.Open();
        LevelsMenu.Open();
    }

}
