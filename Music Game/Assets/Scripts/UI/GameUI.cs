using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class GameUI : MonoBehaviour
    {
        public static event System.Action HintEvent;
        public static event System.Action ReplayEvent;

        [Header("Buttons")]
        [SerializeField] Button pauseButton = null;
        [SerializeField] Button hintButton = null;
        [SerializeField] Button replayButton = null;

        [Header("UI")]
        [SerializeField] TextMeshProUGUI countdownTextGUI = null;
        [SerializeField] TextMeshProUGUI levelText = null;
        [SerializeField] TextMeshProUGUI droneText = null;
        [SerializeField] TextMeshProUGUI gameText = null;
        [SerializeField] TextMeshProUGUI debugText = null;

        [Header("Prefabs")]
        [SerializeField] Button guessButton = null;

        [Header("Containers")]
        [SerializeField] GameObject guessButtonsContainer = null;


        float originalDroneTextSize;

        IEnumerator dronePulseRoutine;

        #region SETUP
        private void Awake()
        {
            Inititialize();
        }
        private void Start()
        {
            pauseButton.onClick.AddListener(OnPausePressed);
            hintButton.onClick.AddListener(OnHintButtonPressed);
            replayButton.onClick.AddListener(OnReplayButtonPressed);
        }
        private void OnDestroy()
        {
            if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPausePressed);
            hintButton.onClick.RemoveListener(OnHintButtonPressed);
            replayButton.onClick.RemoveListener(OnReplayButtonPressed);
        }

        public void Inititialize()
        {
            originalDroneTextSize = droneText.fontSize;
            ShowPauseButton();
            HideHintButton();
            HideReplayButton();
            HideTextGUI(levelText);
            HideTextGUI(countdownTextGUI);
            HideDroneText();
            HideGameText();
            HideDebugText();
        }
        
        public Button InitGuessButton(string noteName)
        {
            if (guessButtonsContainer != null && guessButton != null)
            {
                Button b = Instantiate(guessButton, guessButtonsContainer.transform);
                b.gameObject.SetActive(false);
                b.GetComponent<GuessButton>().Initialize(noteName);
                b.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.GetNoteFormatted(noteName);
                b.onClick.AddListener(b.GetComponent<GuessButton>().ButtonPressed);
                return b;
            }
            return null;
        }
        #endregion

        #region COUNTDOWN
        public void StartCountdown(int startTime, float delay, System.Action endOfCountdownDelegate)
        {
            StartCoroutine(CountdownRoutine(startTime, delay, endOfCountdownDelegate));
        }
        IEnumerator CountdownRoutine(int startTime, float delay, System.Action endOfCountdownDelegate)
        {
            yield return new WaitForSeconds(0.5f); // this delay is needed for smoother start
            ShowTextGUI(countdownTextGUI);
            while (startTime > 0)
            {
                countdownTextGUI.text = startTime.ToString();
                AudioManager.PlaySound(AudioManager.countdown, SoundType.SFX);
                startTime--;
                yield return new WaitForSeconds(delay);
            }
            HideTextGUI(countdownTextGUI);
            endOfCountdownDelegate?.Invoke();
        }

        #endregion

        #region EFFECTS
        public void PulseDroneText()
        {
            StopDronePulse();
            dronePulseRoutine = UIAnimator.PulseTextSizeRoutine(droneText, 1.2f, 0.5f);
            StartCoroutine(dronePulseRoutine);
        }
        public void StopDronePulse()
        {
            if (dronePulseRoutine != null) StopCoroutine(dronePulseRoutine);
        }
        public IEnumerator DisplayCurrentLevelRoutine()
        {
            levelText.text = $"Level {GameManager.Instance.currentLevelIndex + 1}";
            ShowTextGUI(levelText);

            yield return new WaitForSeconds(1f);

            AudioManager.PlaySound(AudioManager.swoosh1, SoundType.SFX);
            UIAnimator.ShrinkToNothing(levelText.rectTransform, 0.5f);
            yield return new WaitForSeconds(1f);
        }
        #endregion

        #region DISPLAY
        public void HideReplayButton() => replayButton.gameObject.SetActive(false);
        public void ShowReplayButton() => replayButton.gameObject.SetActive(true);
        public void HideHintButton() => hintButton.gameObject.SetActive(false);
        public void ShowHintButton() => hintButton.gameObject.SetActive(true);
        public void HidePauseButton() => pauseButton.gameObject.SetActive(false);
        public void ShowPauseButton() => pauseButton.gameObject.SetActive(true);
        public void DisplayGameText(string textToDisplay)
        {
            gameText.text = textToDisplay;
            ShowTextGUI(gameText);
        }
        public void DisplayDroneText(string textToDisplay)
        {
            droneText.fontSize = originalDroneTextSize;
            droneText.text = textToDisplay;
            ShowTextGUI(droneText);
        }
        public void DisplayDebugText(string textToDisplay)
        {
            //debugText.text += textToDisplay + ' ';
            debugText.text += string.Concat(textToDisplay, ' ');
            ShowTextGUI(debugText);
        }
        public void HideGameText() => HideTextGUI(gameText);

        public void HideDroneText() => HideTextGUI(droneText);
        public void HideDebugText()
        {
            HideTextGUI(debugText);
            debugText.text = string.Empty;
        }

        #endregion

        #region BUTTON UTILITY
        public void LoadButtons(List<Button> buttonsList, float timeBetweenButtons = 0f, bool canEnable = true)
        {
            StartCoroutine(LoadButtonsRoutine(buttonsList, timeBetweenButtons, canEnable));
        }
        public IEnumerator LoadButtonsRoutine(List<Button> buttonsList, float timeBetweenButtons, bool canEnable = true)
        {
            for (int i = 0; i < buttonsList.Count; i++)
            {
                LoadButton(buttonsList[i], canEnable, false);
                yield return new WaitForSeconds(timeBetweenButtons);
            }
        }
        public void LoadButton(Button b, bool canEnable = true, bool playSound = true)
        {
            DisplayButton(b, canEnable);
            if (playSound) Utility.AudioManager.PlaySound(Utility.AudioManager.buttonLoad1, Utility.SoundType.SFX);
        }
        public void HideButtons(List<Button> buttonsList)
        {
            foreach (var b in buttonsList)
            {
                HideButton(b);
            }
        }
        public void EnableButtons(List<Button> buttonsList, bool canEnable = true)
        {
            foreach (var b in buttonsList)
            {
                EnableButton(b, canEnable);
            }
        }
        public void DisableButtons(List<Button> buttonsList)
        {
            EnableButtons(buttonsList, false);
        }

        #endregion

        #region BUTTON EVENTS
        void OnPausePressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            GameManager.ChangeGameState(GameState.Paused);
        }
        void OnHintButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(hintButton, AudioManager.buttonSelect2);
            HintEvent?.Invoke();

        }
        void OnReplayButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(replayButton, AudioManager.buttonSelect2);
            ReplayEvent?.Invoke();
        }
        #endregion

        #region HELPER METHODS
        void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true) => textGUI.gameObject.SetActive(canShow);
        void HideTextGUI(TextMeshProUGUI textGUI) => ShowTextGUI(textGUI, false);
        void EnableButton(Button b, bool isInteractable = true)
        {
            if (b) b.interactable = isInteractable;
        }
        void DisplayButton(Button b, bool canEnable = true)
        {
            if (b)
            {
                EnableButton(b, canEnable);
                b.gameObject.SetActive(true);
            }
        }
        void HideButton(Button b)
        {
            if (b) b.gameObject.SetActive(false);
        }

        #endregion
        
        
    }
}

