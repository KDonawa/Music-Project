using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class MainMenu : Menu<MainMenu>
    {
        [SerializeField] Button newGameButton = null;
        [SerializeField] Button continueButton = null;
        [SerializeField] Button settingsButton = null;
        [SerializeField] Button quitButton = null;        

        [Header("New Game Tooltip")]        
        [SerializeField] GameObject newGameTooltip = null;
        [SerializeField] Button yesButton = null;
        [SerializeField] Button noButton = null;

        protected override void Awake()
        {
            base.Awake();

            newGameButton.onClick.AddListener(() =>
            {
                if (GameManager.Instance.isNewGame)
                {
                    UIAnimator.ButtonPressEffect3(newGameButton, AudioManager.buttonSelect1);
                    StartNewGame();
                }
                else
                {
                    UIAnimator.ButtonPressEffect3(newGameButton, AudioManager.buttonSelect2);
                    newGameTooltip.SetActive(true);
                }                   
            });
            yesButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(yesButton, AudioManager.buttonSelect1);
                StartNewGame();
            });
            noButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(noButton, AudioManager.buttonSelect2);
                newGameTooltip.SetActive(false);
            });
            continueButton.onClick.AddListener(() =>
            {
                GameManager.LoadGame();
                UIAnimator.ButtonPressEffect3(continueButton, AudioManager.buttonSelect1);
                SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, StageSelectMenu.Instance.Open);
            });
            settingsButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(settingsButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, SettingsMenu.Instance.Open);
            });
            quitButton.onClick.AddListener(() =>
            {
                UIAnimator.ButtonPressEffect3(quitButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, GameManager.QuitGame);
            });
        }
        
        public override void Open()
        {
            continueButton.gameObject.SetActive(!GameManager.Instance.isNewGame);
            newGameTooltip.SetActive(false);
            base.Open();
        }
        void StartNewGame()
        {
            GameManager.StartNewGame();
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, StageSelectMenu.Instance.Open);
        }
    }
}

