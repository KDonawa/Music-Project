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
        [SerializeField] GameObject buttonsContainer = null;
        [SerializeField] Button unlockedButtonPrefab = null;
        [SerializeField] Button lockedButtonPrefab = null;

        [Header("HiScore Panel")]
        [SerializeField] Button hiScoresButton = null;
        [SerializeField] GameObject hiScorePanel = null;
        [SerializeField] GameObject hiScoreSlotsContainer = null;
        [SerializeField] HiScoreSlot hiScoreSlotPrefab = null;
        [SerializeField] Button closeButton = null;
        List<HiScoreSlot> _hiScoreSlots = new List<HiScoreSlot>();

        List<Button> levelOptions;

        protected override void Awake()
        {
            base.Awake();

            levelOptions = new List<Button>();

            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
            });
            if (backButton != null) backButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, StageSelectMenu.Instance.Open);
            });
            if (hiScoresButton != null) hiScoresButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(hiScoresButton, AudioManager.buttonSelect2);
                OpenHiScorePanel();
            });
            closeButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(closeButton, AudioManager.buttonSelect2);
                hiScorePanel.SetActive(false);
            });

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
                        lsb.InitializeButton(i, levels[i].numStarsEarned);
                        b.onClick.AddListener(() => lsb.ButtonPressed(ButtonPressedEffect));
                    }
                    levelOptions.Add(b);
                }
            }
        }
        void OpenHiScorePanel()
        {
            if (hiScorePanel == null) return;

            LevelData[] levels = GameManager.GetCurrentStage().levels;
            int i = 0;
            for (; i < levels.Length && i < _hiScoreSlots.Count; i++)
            {
                if (levels[i] != null)
                {
                    _hiScoreSlots[i].SetLevelNumberText(i + 1);
                    _hiScoreSlots[i].SetHiScoreText(levels[i].hiScore);
                    _hiScoreSlots[i].gameObject.SetActive(true);
                }
            }
            for (; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    _hiScoreSlots.Add(Instantiate(hiScoreSlotPrefab, hiScoreSlotsContainer.transform));
                    _hiScoreSlots[i].SetLevelNumberText(i + 1);
                    _hiScoreSlots[i].SetHiScoreText(levels[i].hiScore);
                }
            }
            for (; i < _hiScoreSlots.Count; i++) _hiScoreSlots[i].gameObject.SetActive(false);

            hiScorePanel.SetActive(true);
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

        #endregion
    }
}

