using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuGeneric<MainMenu>
{  
    public void OnPlayPressed()
    {
        StageSelectMenu.Open();
    }
    public void OnSettingsPressed()
    {
        SettingsMenu.Open();
    }
    public void OnQuitPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
