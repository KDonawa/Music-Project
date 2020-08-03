using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : MenuGeneric<LevelCompleteMenu>
{
    [SerializeField] TextMeshProUGUI scoreText = null;
    [SerializeField] Button nextLevelButton = null;

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsFinalLevel())
        {
            nextLevelButton.gameObject.SetActive(false);
        }
        else nextLevelButton.gameObject.SetActive(true);
    }
    public void SetFinalScore(float score)
    {
        scoreText.text = score.ToString() + "%";
    }
    public void OnRestartPressed()
    {
        //if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();

        LevelGameplay gameplay = FindObjectOfType<LevelGameplay>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.CLOSE_HORIZONTAL, OutTransition.OPEN_VERTICAL, gameplay.RestartLevel);
        }
    }

    public void OnNextLevelPressed()
    {
        //if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();

        LevelGameplay gameplay = FindObjectOfType<LevelGameplay>();
        if (gameplay != null)
        {
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, gameplay.PlayNextLevel);
        }
    }

    public void OnHomeButtonPressed()
    {      
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.CIRCLE_WIPE_UP, LoadLevelsMenu);
    }
    void LoadLevelsMenu()
    {
        if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();
        GameManager.LoadStartScene();
        LevelsMenu.Open();
    }
    // main menu

    // level select
}
