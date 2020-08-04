using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MenuGeneric<GameMenu>
{
    [Header("References")]
    [SerializeField] Button pauseButton = null;

    private void Start()
    {
        if (pauseButton == null) Debug.LogError("No reference to Pause button");
        else pauseButton.onClick.AddListener(OnPausePressed);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPausePressed);
    }

    void OnPausePressed()
    {
        UIAnimator.ButtonPressEffect(pauseButton, AudioManager.click1);
        Time.timeScale = 0f;
        PauseMenu.Open();

        LevelGameplay level = FindObjectOfType<LevelGameplay>();
        if (level)
        {
            level.PauseGame();
        }      
    }
}
