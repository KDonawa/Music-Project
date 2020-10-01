using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;
using KD.MusicGame.Utility.SaveSystem;
using KD.MusicGame.Gameplay;

namespace KD.MusicGame.UI
{
    public class StageSelectMenu : Menu<StageSelectMenu>
    {
        #region SETUP
        [Header("UI/Prefabs")]
        [SerializeField] GameObject buttonsContainer = null;

        [SerializeField] Button backButton = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button createCustomStageButton = null;

        [SerializeField] Button unlockedButtonPrefab = null;
        [SerializeField] Button lockedButtonPrefab = null;

        [SerializeField] StageCreationScreen creationScreen = null;


        List<Button> stageOptions;

        protected override void Awake()
        {
            base.Awake();

            stageOptions = new List<Button>();
            if (creationScreen != null) creationScreen.gameObject.SetActive(false);

            if (backButton != null) backButton.onClick.AddListener(BackPressed);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(MainMenuPressed);
            if (createCustomStageButton != null) createCustomStageButton.onClick.AddListener(CreateCustomStagePressed);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
            if (createCustomStageButton != null) createCustomStageButton.onClick.RemoveListener(CreateCustomStagePressed);
        }
        public override void Open()
        {
            InitializeMenu();
            base.Open();
        }

        public void InitializeMenu()
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;            

            foreach (var b in stageOptions) Destroy(b.gameObject);
            stageOptions.Clear();

            List<StageData> stages = GameManager.Instance.stagesList;
            for (int i = 0; i < stages.Count; i++)
            {
                Button b;
                if (!stages[i].isUnlocked) b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
                else b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);

                StageSelectButton ssb = b.GetComponent<StageSelectButton>();
                ssb.Init(i + 1, stages[i]);

                if (stages[i].isUnlocked) b.onClick.AddListener(() => ssb.ButtonPressed(ButtonPressedEffect));

                stageOptions.Add(b);
            }

            List<StageData> customStages = GameManager.Instance.customStagesList;
            for (int i = 0; i < customStages.Count; i++)
            {
                Button b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);
                customStages[i].name = $"Custom {i + 1}";
                StageSelectButton ssb = b.GetComponent<StageSelectButton>();
                ssb.Init(i + 1 + stages.Count, customStages[i]);
                b.onClick.AddListener(() => ssb.ButtonPressed(ButtonPressedEffect));

                stageOptions.Add(b);
            }


        }
        #endregion

        #region BUTTON EVENTS
        void CreateCustomStagePressed()
        {
            UIAnimator.ButtonPressEffect3(createCustomStageButton, AudioManager.buttonSelect2);
            if (creationScreen == null || GameManager.Instance.customStagesList.Count >= 4) return;

            creationScreen.gameObject.SetActive(true);
        }
        void ButtonPressedEffect(Button button)
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = false;
            foreach (var b in stageOptions)
            {
                if (b != button)
                {
                    RectTransform rect = b.GetComponent<RectTransform>();
                    UIAnimator.ShrinkToNothing(rect, 0.5f, 2f);
                }
            }
        }
        void BackPressed()
        {
            UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, MainMenu.Instance.Open);
        }
        void MainMenuPressed()
        {
            UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
        }
        

        #endregion
    }
}

