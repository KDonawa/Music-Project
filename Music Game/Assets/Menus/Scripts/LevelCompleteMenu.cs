using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompleteMenu : MenuTemplpate<LevelCompleteMenu>
{
    [SerializeField] TextMeshProUGUI scoreText = null;
    public void SetFinalScore(float score)
    {
        scoreText.text = score.ToString();
    }
    public void OnRestartPressed()
    {
        if (MenuManager.Instance) MenuManager.Instance.ClearMenuHistory();
        LevelGameplayUtility gameplayUtility = FindObjectOfType<LevelGameplayUtility>();
        if (gameplayUtility != null) gameplayUtility.RestartGame();
    }

    // main menu

    // level select
}
