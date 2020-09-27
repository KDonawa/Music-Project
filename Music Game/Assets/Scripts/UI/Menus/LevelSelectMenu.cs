using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility.SaveSystem;
using KD.MusicGame.Utility;
using KD.MusicGame.Gameplay;

namespace KD.MusicGame.UI
{
    public class LevelSelectMenu : Menu<LevelSelectMenu>
    {
        [Header("UI")]
        [SerializeField] TextMeshProUGUI headerText = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button backButton = null;
        [SerializeField] Button hiScoresButton = null;
        [SerializeField] GameObject buttonsContainer = null;
        [SerializeField] HiScorePanel hiScorePanel = null;

        [Header("Prefabs")]
        [SerializeField] Button unlockedButtonPrefab = null;
        [SerializeField] Button lockedButtonPrefab = null;

        List<Button> levelOptions;

        protected override void Awake()
        {
            base.Awake();

            levelOptions = new List<Button>();

            if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
            else mainMenuButton.onClick.AddListener(MainMenuPressed);

            if (backButton == null) Debug.LogError("No reference to back button");
            else backButton.onClick.AddListener(BackPressed);
            if (hiScoresButton != null) hiScoresButton.onClick.AddListener(OnHiScoresButtonPressed);

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
            if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
            if (hiScoresButton != null) hiScoresButton.onClick.RemoveListener(OnHiScoresButtonPressed);
        }
        public override void Open()
        {
            InitializeMenu();
            base.Open();
        }
        
        void InitializeMenu()
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;
            if (hiScorePanel != null) hiScorePanel.gameObject.SetActive(false);
            headerText.text = GameManager.GetCurrentStage().name;

            foreach (var b in levelOptions) Destroy(b.gameObject);
            levelOptions.Clear();

            //Gameplay.Level[] levels = GameManager.CurrentLevels;
            LevelData[] levels = GameManager.GetCurrentStage().levels;
            for (int i = 0; i < levels.Length; i++)
            {
                if(levels[i] != null)
                {
                    Button b;
                    if (!levels[i].isUnlocked) b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
                    else
                    {
                        b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);
                        LevelSelectButton lsb = b.GetComponent<LevelSelectButton>();
                        lsb.InitializeButton(i + 1, levels[i].numStarsEarned);
                        b.onClick.AddListener(() => lsb.ButtonPressed(ButtonPressedEffect));
                    }
                    levelOptions.Add(b);
                }
            }
        }

        #region BUTTON EVENTS
        void ButtonPressedEffect(Button button)
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (var b in levelOptions)
            {
                if (b != button)
                {
                    RectTransform rect = b.GetComponent<RectTransform>();
                    UIAnimator.ShrinkToNothing(rect, 0.5f, 2f);
                }
            }
        }
        void MainMenuPressed()
        {
            UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
        }
        void BackPressed()
        {
            UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, StageSelectMenu.Instance.Open);
        }
        void OnHiScoresButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(hiScoresButton, AudioManager.buttonSelect2);
            if (hiScorePanel != null) hiScorePanel.Open();
        }
        #endregion
    }
}

