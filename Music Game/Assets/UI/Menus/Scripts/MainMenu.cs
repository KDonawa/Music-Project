using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuGeneric<MainMenu>
{  
    public void OnPlayPressed()
    {
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, StageSelectMenu.Open);
    }
    public void OnSettingsPressed()
    {
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, SettingsMenu.Open);
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
