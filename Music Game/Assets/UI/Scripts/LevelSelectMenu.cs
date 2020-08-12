using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility.SaveSystem;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class LevelSelectMenu : Menu<LevelSelectMenu>
    {
        [Header("UI")]
        [SerializeField] TextMeshProUGUI headerText = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button backButton = null;
        [SerializeField] GameObject buttonsContainer = null;

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

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
            if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
        }
        public override void Open()
        {
            InitializeMenu();
            base.Open();
        }
        void LoadData()
        {
            LevelSaveData data = BinarySaveSystem.LoadLevelData(GameManager.CurrentStageIndex);
            Gameplay.Level[] levels = GameManager.CurrentLevels;
            if (levels != null && data != null)
            {
                for (int i = 0; i < levels.Length && i < data.unlockedLevels.Length; i++)
                {
                    levels[i].isUnlocked = data.unlockedLevels[i];
                    levels[i].numStarsEarned = data.starsEarned[i];
                }
            }
        }

        void InitializeMenu()
        {
            LoadData();

            headerText.text = GameManager.CurrentStage.name;

            foreach (var b in levelOptions) Destroy(b.gameObject);
            levelOptions.Clear();

            Gameplay.Level[] levels = GameManager.CurrentLevels;
            if (levels == null) return;

            for (int i = 1; i <= GameManager.CurrentLevels.Length; i++)
            {
                Button b;

                if (i == 1) levels[i - 1].isUnlocked = true;
                if (levels[i - 1].isUnlocked) b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);
                else b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
                b.interactable = levels[i - 1].isUnlocked;
                levelOptions.Add(b);

                LevelSelectButton lsb = b.GetComponent<LevelSelectButton>();
                lsb.InitializeButton(i, levels[i - 1].numStarsEarned);
                b.onClick.AddListener(() => lsb.ButtonPressed(ButtonPressedEffect));
            }
        }
        void ButtonPressedEffect(Button button)
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (var b in levelOptions)
            {
                if (b != button)
                {
                    RectTransform rect = b.GetComponent<RectTransform>();
                    UIAnimator.ShrinkToNothing(rect, 0.5f, 2f, () => ButtonPressEffectCompleted(rect));
                }
            }
        }
        void ButtonPressEffectCompleted(RectTransform rect)
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;
            rect.gameObject.SetActive(true);
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

    }
}

