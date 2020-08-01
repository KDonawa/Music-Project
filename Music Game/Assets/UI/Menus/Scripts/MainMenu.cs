using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MenuGeneric<MainMenu>
{  
    public void OnPlayPressed()
    {
        SceneTransitions.PlaySceneTransition(SceneTransitions.CIRCLE_FILL, StageSelectMenu.Open);
    }
    public void OnSettingsPressed()
    {
        SceneTransitions.PlaySceneTransition(SceneTransitions.CROSS_FADE, SettingsMenu.Open);
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
