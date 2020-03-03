using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsMenu : MenuTemplpate<LevelsMenu>
{
    public override void OnBackPressed()
    {
        base.OnBackPressed();
    }
    public virtual void OnMainMenuPressed()
    {
        MainMenu.Open();
    }

    public void LoadLevel(int levelIndex)
    {
        SceneLoader.LoadScene(levelIndex);
        MenuManager.Instance.ClearMenuHistory();
    }
}
