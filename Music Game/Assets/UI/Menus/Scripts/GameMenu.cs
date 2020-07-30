using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MenuGeneric<GameMenu>
{
    public void OnPausePressed()
    {
        Time.timeScale = 0f;
        PauseMenu.Open();

        LevelGameplay level = FindObjectOfType<LevelGameplay>();
        if (level)
        {
            level.PauseGame();
        }      
    }
}
