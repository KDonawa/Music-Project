using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;
using KD.MusicGame.Utility.SaveSystem;

namespace KD.MusicGame.UI
{
    public class StageSelectMenu : Menu<StageSelectMenu>
    {
        [Header("References")]
        [SerializeField] GameObject buttonsContainer = null;
        [SerializeField] Button backButton = null;
        [SerializeField] Button mainMenuButton = null;

        [Header("UI/Prefabs")]
        [SerializeField] Button unlockedButtonPrefab = null;
        [SerializeField] Button lockedButtonPrefab = null;


        List<Button> stageOptions;

        protected override void Awake()
        {
            base.Awake();

            stageOptions = new List<Button>();

            if (backButton == null) Debug.LogError("No reference to back button");
            else backButton.onClick.AddListener(BackPressed);

            if (mainMenuButton == null) Debug.LogError("No reference to Main Menu button");
            else mainMenuButton.onClick.AddListener(MainMenuPressed);

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
            if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(MainMenuPressed);
        }
        public override void Open()
        {
            InitializeMenu();
            base.Open();
        }

        void InitializeMenu()
        {
            buttonsContainer.GetComponent<GridLayoutGroup>().enabled = true;            

            foreach (var b in stageOptions) Destroy(b.gameObject);
            stageOptions.Clear();

            Gameplay.Stage[] stages = GameManager.Stages;
            for (int i = 0; i < stages.Length; i++)
            {
                if(stages[i] != null)
                {
                    Button b;
                    if (!stages[i].isUnlocked) b = Instantiate(lockedButtonPrefab, buttonsContainer.transform);
                    else
                    {
                        b = Instantiate(unlockedButtonPrefab, buttonsContainer.transform);
                        StageSelectButton ssb = b.GetComponent<StageSelectButton>();
                        ssb.Init(i + 1, stages[i].name, stages[i].numPassedLevels);
                        b.onClick.AddListener(() => ssb.ButtonPressed(ButtonPressedEffect));
                    }
                    stageOptions.Add(b);
                }                
            }
        }

        #region BUTTON EVENTS
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
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_RIGHT, OutTransition.CIRCLE_WIPE_RIGHT, DroneSelectMenu.Instance.Open);
        }
        void MainMenuPressed()
        {
            UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_DOWN, OutTransition.CIRCLE_WIPE_DOWN, MainMenu.Instance.Open);
        }

        #endregion
    }
}

