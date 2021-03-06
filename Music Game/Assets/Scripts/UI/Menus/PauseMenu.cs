﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class PauseMenu : Menu<PauseMenu>
    {
        [Header("Buttons")]
        [SerializeField] Button resumeButton = null;
        [SerializeField] Button restartButton = null;
        [SerializeField] Button settingsButton = null;
        [SerializeField] Button exitButton = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button stageSelectButton = null;

        protected override void Awake()
        {
            base.Awake();

            if (resumeButton == null) Debug.LogError("No reference to resume button");
            else resumeButton.onClick.AddListener(OnResumePressed);

            if (restartButton == null) Debug.LogError("No reference to restart button");
            else restartButton.onClick.AddListener(OnRestartPressed);

            if (settingsButton == null) Debug.LogError("No reference to settings button");
            else settingsButton.onClick.AddListener(OnSettingsPressed);

            if (exitButton == null) Debug.LogError("No reference to exit button");
            else exitButton.onClick.AddListener(OnExitPressed);

            if (mainMenuButton == null) Debug.LogError("No reference to main menu button");
            else mainMenuButton.onClick.AddListener(OnMainMenuPressed);

            if (stageSelectButton == null) Debug.LogError("No reference to stage button");
            else stageSelectButton.onClick.AddListener(OnStageSelectPressed);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumePressed);
            if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsPressed);
            if (exitButton != null) exitButton.onClick.RemoveListener(OnExitPressed);
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(OnMainMenuPressed);
            if (stageSelectButton != null) stageSelectButton.onClick.RemoveListener(OnStageSelectPressed);
        }
        void OnMainMenuPressed()
        {
            GameManager.ChangeGameState(GameState.Running);
            UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
            Gameplay.Game.Stop();
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, () => GameManager.LoadStartScene());
        }
        void OnStageSelectPressed()
        {
            GameManager.ChangeGameState(GameState.Running);
            UIAnimator.ButtonPressEffect3(stageSelectButton, AudioManager.buttonSelect2);
            Gameplay.Game.Stop();
            SceneTransitions.PlayTransition
                (InTransition.CIRCLE_EXPAND, OutTransition.OPEN_HORIZONTAL, () => GameManager.LoadStartScene(StageSelectMenu.Instance.Open));
        }
        void OnResumePressed()
        {
            GameManager.ChangeGameState(GameState.Running);
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            Close();
        }
        void OnRestartPressed()
        {
            GameManager.ChangeGameState(GameState.Running);
            UIAnimator.ButtonPressEffect3(restartButton, AudioManager.buttonSelect1);
            Gameplay.Game.Stop();
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, Gameplay.Game.Play);
        }
        void OnSettingsPressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            SettingsMenu.Instance.Open();
        }
        void OnExitPressed()
        {
            GameManager.ChangeGameState(GameState.Running);
            UIAnimator.ButtonPressEffect3(exitButton, AudioManager.buttonSelect2);
            Gameplay.Game.Stop();
            SceneTransitions.PlayTransition
                (InTransition.CIRCLE_EXPAND, OutTransition.OPEN_VERTICAL, () => GameManager.LoadStartScene(LevelSelectMenu.Instance.Open));
        }
    }
}

