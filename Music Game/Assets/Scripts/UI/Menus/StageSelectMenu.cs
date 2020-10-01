using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;
using KD.MusicGame.Gameplay;
using TMPro;
using KD.MusicGame.Utility.SaveSystem;

namespace KD.MusicGame.UI
{
    public class StageSelectMenu : Menu<StageSelectMenu>
    {
        #region SETUP     
        [SerializeField] GameObject buttonsContainer = null;
        [SerializeField] Button backButton = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button unlockedButtonPrefab = null;
        [SerializeField] Button lockedButtonPrefab = null;
        [SerializeField] GameObject tooltip = null;

        [Header("Custom Stages")]
        [Range(0, 4)] [SerializeField] int maxNumCustomStages = 4;
        [SerializeField] StageCreationScreen creationScreen = null;
        [SerializeField] Button createCustomStageButton = null;
        [SerializeField] Button customButtonPrefab = null;
        [SerializeField] GameObject customButtonSelectedTooltip = null;
        [SerializeField] Button playCustomStageButton = null;
        [SerializeField] Button deleteCustomStageButton = null;
        [SerializeField] Button closeCustomStageToolipButton = null;

        List<Button> stageButtons;
        CustomStageSelectButton _activeButton;

        protected override void Awake()
        {
            base.Awake();

            stageButtons = new List<Button>();
            if (creationScreen != null) creationScreen.gameObject.SetActive(false);
            if (customButtonSelectedTooltip != null) customButtonSelectedTooltip.SetActive(false);
            

            // init button events
            if (backButton != null) backButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, MainMenu.Instance.Open);
            });
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
            });
            if (createCustomStageButton != null) createCustomStageButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(createCustomStageButton, AudioManager.buttonSelect2);
                if (creationScreen == null || GameManager.customStagesList.Count >= maxNumCustomStages)
                {
                    if (tooltip != null)
                    {
                        tooltip.GetComponentInChildren<TextMeshProUGUI>().text = $"Cannot create more than {maxNumCustomStages} Custom Stages";
                        tooltip.gameObject.SetActive(true);
                    }                    
                    return;
                }
                creationScreen.gameObject.SetActive(true);
            });
            if (playCustomStageButton) playCustomStageButton.onClick.AddListener(() =>
            {
                SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_VERTICAL, LevelSelectMenu.Instance.Open);
                UIAnimator.ButtonPressEffect1(playCustomStageButton, AudioManager.buttonSelect1);
                customButtonSelectedTooltip.gameObject.SetActive(false);                
                StageButtonPressed();
            });
            if (deleteCustomStageButton) deleteCustomStageButton.onClick.AddListener(() =>
            {
                if (_activeButton == null) return;
                UIAnimator.ButtonPressEffect1(deleteCustomStageButton, AudioManager.buttonSelect2);
                GameManager.customStagesList.Remove(_activeButton.stage);
                BinarySaveSystem.SaveCustomGameData();
                InitializeMenu();
                customButtonSelectedTooltip.gameObject.SetActive(false);
            });
            if (closeCustomStageToolipButton) closeCustomStageToolipButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect1(closeCustomStageToolipButton, AudioManager.buttonSelect2);
                customButtonSelectedTooltip.gameObject.SetActive(false);
            });
        }       

        public override void Open()
        {
            InitializeMenu();
            base.Open();
        }

        public void InitializeMenu()
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;
            if (tooltip != null) tooltip.SetActive(false);

            _activeButton = null;

            foreach (var b in stageButtons) Destroy(b.gameObject);
            stageButtons.Clear();

            List<StageData> stages = GameManager.stagesList;
            for (int i = 0; i < stages.Count; i++)
            {
                Button b;
                if (!stages[i].isUnlocked) b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
                else b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);

                StageSelectButton ssb = b.GetComponent<StageSelectButton>();
                ssb.Init(i, stages[i]);

                if (stages[i].isUnlocked) b.onClick.AddListener(() =>
                {
                    ssb.ButtonPressed();
                    StageButtonPressed();
                });

                stageButtons.Add(b);
            }

            List<StageData> customStages = GameManager.customStagesList;
            for (int i = 0; i < customStages.Count; i++)
            {
                Button b = Instantiate(customButtonPrefab, buttonsContainer.transform);
                customStages[i].name = $"Custom {i + 1}";
                CustomStageSelectButton cssb = b.GetComponent<CustomStageSelectButton>();
                cssb.Init(i + stages.Count, customStages[i]);
                b.onClick.AddListener(() =>
                {
                    _activeButton = cssb;
                    customButtonSelectedTooltip.gameObject.SetActive(true);
                    cssb.ButtonPressed();
                });
                stageButtons.Add(b);
            }
        }
        #endregion

        #region BUTTON EFFECTS
        void StageButtonPressed()
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (var b in stageButtons)
            {
                UIAnimator.ShrinkToNothing(b.GetComponent<RectTransform>(), 0.5f, 2f);
            }
        }
        #endregion
    }
}

