using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuGeneric<MainMenu>
{  
    public void OnPlayPressed()
    {
        //SceneTransitions.PlayTransition(SceneTransitions.CROSS_FADE, StageSelectMenu.Open);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, StageSelectMenu.Open);
    }
    public void OnSettingsPressed()
    {
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, SettingsMenu.Open);
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
