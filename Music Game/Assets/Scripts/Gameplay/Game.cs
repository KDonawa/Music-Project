using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;
using KD.MusicGame.Utility.SaveSystem;
using KD.MusicGame.UI;
/*
IDEAS:     
-hints reduce score by 10% each time one is used: add a variable to keep track of hints/replays
-instrument select screen    
*/
namespace KD.MusicGame.Gameplay
{
    public class Game : MonoBehaviour
    {
        static Game _instance;

        public static event System.Action LevelPassedEvent;

        #region SETUP
        [Header("Prefabs")]
        [SerializeField] GameUI gameUI = null;

        [Header("Variables")]
        [Range(0, 100)] [SerializeField] int levelPassPercentage = 50;
        [Range(1, 10)] [SerializeField] int numRoundsPerSublevel = 5;

        [Header("Systems")]
        [SerializeField] ScoreSystem scoreSystemPrefab = null;
        [SerializeField] TextSystem textSystemPrefab = null;

        // refs
        GameUI _gameUI;
        ScoreSystem _scoreSystem;
        TextSystem _textSystem;

        // variables
        LevelData currentLevel;
        List<Button> guessButtons;
        List<string> currentNotes;
        string droneNote;
        int numGuessesPerRound;
        int currentSubLevelIndex;
        int guessCount;
        int currentRound;
        List<string> answers;
        List<string> activeNoteSounds;       

        
        private void Awake()
        {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);

            _gameUI = Instantiate(gameUI);
            _scoreSystem = Instantiate(scoreSystemPrefab);
            _textSystem = Instantiate(textSystemPrefab);

            guessButtons = new List<Button>();
            currentNotes = new List<string>();
            droneNote = string.Empty;
            answers = new List<string>();
            activeNoteSounds = new List<string>();
        }
        private void Start()
        {
            GuessButton.ButtonPressedEvent += OnGuessButtonPressed;
            GuessButton.GuessCheckedEvent += OnGuessButtonChecked;

            GameManager.GamePausedEvent += OnGamePaused;
            GameManager.GameUnPausedEvent += OnGameResumed;

            GameUI.HintEvent += ShowHint;
            GameUI.ReplayEvent += PlayNotes;

            GuessButton.GuessIncorrectEvent += ShowCorrectGuessOnWrongGuess;

            Play();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;

            GameManager.GamePausedEvent -= OnGamePaused;
            GameManager.GameUnPausedEvent -= OnGameResumed;

            GuessButton.ButtonPressedEvent -= OnGuessButtonPressed;
            GuessButton.GuessCheckedEvent -= OnGuessButtonChecked;

            GameUI.HintEvent -= ShowHint;
            GameUI.ReplayEvent -= PlayNotes;

            GuessButton.GuessIncorrectEvent -= ShowCorrectGuessOnWrongGuess;
        }

        void InitializeLevel()
        {
            MenuManager.CloseAllMenus();
            AudioManager.StopAllNoteSounds();
            _gameUI.HideButtons(guessButtons);

            currentLevel = GameManager.GetCurrentLevelData();
            currentSubLevelIndex = 0;
            numGuessesPerRound = currentLevel.numNotesToGuess;

            _gameUI.Inititialize();
            _scoreSystem.Initialize(numGuessesPerRound * numRoundsPerSublevel * currentLevel.subLevels.Length);
            _textSystem.Initialize();
        }
        void InitializeNotes()
        {
            droneNote = GameManager.Instance.droneNote;
            currentNotes.Clear();
            foreach (var note in currentLevel.subLevels[currentSubLevelIndex]) currentNotes.Add(note);
        }
        void InitializeGuessButtons()
        {
            foreach (var b in guessButtons) Destroy(b.gameObject);
            guessButtons.Clear();
            foreach (var note in currentNotes) guessButtons.Add(_gameUI.InitGuessButton(note));
        }
        #endregion

        #region GAMEPLAY

        public static void Play()
        {
            _instance.InitializeLevel(); 
            _instance.StartCoroutine(_instance.StartLevelRoutine());
        }
        IEnumerator StartLevelRoutine()
        {
            while (!SceneTransitions.sceneLoadingComplete) yield return null; //wait until scene is loaded

            yield return StartCoroutine(_gameUI.DisplayCurrentLevelRoutine());
            PlayGameLoop();
        }
        void PlayGameLoop()
        {
            InitializeNotes();
            InitializeGuessButtons();
            currentRound = 1;
            PlayRound();
        }
        void PlayRound()
        {
            guessCount = 0;
            answers.Clear();
            activeNoteSounds.Clear();

            StartCoroutine(_gameUI.LoadButtonsRoutine(guessButtons, 0.2f, false));

            string numNotesText = numGuessesPerRound > 1 ? " Notes" : " Note";
            _gameUI.DisplayGameText("Guess " + numGuessesPerRound + numNotesText);

            for (int i = 0; i < numGuessesPerRound; i++)
            {
                answers.Add(currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)]);
            }
            StartCoroutine(PlayNotesRoutine());
        }
        
        IEnumerator PlayNotesRoutine()
        {
            _gameUI.HideHintButton();
            _gameUI.HideReplayButton();
            _gameUI.DisableButtons(guessButtons);

            yield return new WaitForSeconds(1.5f);
            PlayDroneNoteEffect();
            yield return new WaitForSeconds(2.1f);

            // play each note consecutively
            for (int i = 0; i < numGuessesPerRound; i++)
            {               
                PlayInstrumentNote(GameManager.GetWesternNotation(answers[i], droneNote));
                //_gameUI.DisplayDebugText(_utility.GetNoteFormatted(note)); // testing

                //yield return new WaitForSeconds(1.8f);
                yield return new WaitForSeconds(4f - SettingsMenu.NoteSpeedSlider.value);
                StopInstrumentNote(GameManager.GetWesternNotation(answers[i], droneNote));
            }

            yield return new WaitForSeconds(0.2f);
            StopDroneNoteEffect();
            yield return new WaitForSeconds(0.2f);
            _gameUI.ShowHintButton();
            _gameUI.ShowReplayButton();
            _gameUI.HideDroneText();
            _gameUI.EnableButtons(guessButtons);
        }
        IEnumerator ContinueGameLoop()
        {
            currentSubLevelIndex++;
            _gameUI.HideHintButton();
            _gameUI.HideReplayButton();          
            yield return new WaitForSeconds(1f);
            _gameUI.HideButtons(guessButtons);
            _gameUI.HideGameText();
            _gameUI.HideDebugText();
            yield return new WaitForSeconds(1f);

            if (HasLevelEnded())
            {
                _gameUI.HidePauseButton();

                if (IsLevelPassed())
                {
                    LevelPassedEvent?.Invoke();
                    if (!currentLevel.isPassed)
                    {
                        currentLevel.isPassed = true;
                        GameManager.GetCurrentStage().numPassedLevels += 1;
                    }
                }
                int finalScore = _scoreSystem.FinalScorePercentage();
                int currentHiScore = currentLevel.hiScore;
                if (finalScore > currentHiScore) currentLevel.hiScore = finalScore;

                // calculate stars earned
                int numStars = 0;
                if (finalScore >= 50) numStars = 1;
                if (finalScore >= 75) numStars = 2;
                if (finalScore == 100) numStars = 3;
                if (numStars > currentLevel.numStarsEarned) currentLevel.numStarsEarned = numStars;

                // save progress
                BinarySaveSystem.SaveGameData();
                BinarySaveSystem.SaveCustomGameData();

                LevelCompleteMenu.DisplayMenu(finalScore, IsLevelPassed(), currentHiScore, numStars);
            }                         
            else PlayGameLoop();
        }

        bool HasLevelEnded() => currentSubLevelIndex == currentLevel.subLevels.Length;
        bool IsLevelPassed() => _scoreSystem.FinalScorePercentage() >= levelPassPercentage;
        #endregion

        #region EVENTS
        void ShowHint() => StartCoroutine(ShowHintRoutine());
        void PlayNotes() => StartCoroutine(PlayNotesRoutine());
        IEnumerator ShowHintRoutine()
        {
            _gameUI.HideHintButton();
            _gameUI.HideReplayButton();
            _gameUI.DisableButtons(guessButtons);
            for (int i = 0; i < currentNotes.Count; i++)
            {
                Button b = guessButtons[i];
                _gameUI.LoadButton(b, false, false);
                PlayInstrumentNote(GameManager.GetWesternNotation(currentNotes[i], droneNote));
                yield return new WaitForSeconds(0.3f);
                UIAnimator.FlashButtonColor(b, Color.green, 2f);

                yield return new WaitForSeconds(1.5f);
                StopInstrumentNote(GameManager.GetWesternNotation(currentNotes[i], droneNote));
            }

            yield return new WaitForSeconds(0.2f);
            _gameUI.EnableButtons(guessButtons);
            _gameUI.ShowHintButton();
            _gameUI.ShowReplayButton();
        }
        void OnTimerExpired() => StartCoroutine(TimerExpiredRoutine());
        IEnumerator TimerExpiredRoutine()
        {
            _gameUI.DisableButtons(guessButtons);

            AudioManager.PlaySound(AudioManager.timerExpired, SoundType.SFX);

            yield return new WaitForSeconds(1f);
            _gameUI.HideButtons(guessButtons);
            yield return new WaitForSeconds(1f);
            StartCoroutine(ContinueGameLoop());
        }
        void ShowCorrectGuessOnWrongGuess()
        {
            foreach (var button in guessButtons)
            {
                button.GetComponent<GuessButton>().ShowCorrectGuess();
            }
        }
        void OnGuessButtonPressed(GuessButton guessButton)
        {
            _gameUI.DisableButtons(guessButtons);            
            GuessButton.correctGuess = answers[guessCount++];
        }
        void OnGuessButtonChecked()
        {
            if (guessCount == numGuessesPerRound)
            {
                currentRound++;
                if (currentRound > numRoundsPerSublevel) StartCoroutine(ContinueGameLoop());
                else PlayRound();
            }
            else
            {
                _gameUI.EnableButtons(guessButtons);
            }
        }
        void OnGamePaused()
        {
            AudioManager.PauseSound(droneNote, SoundType.DRONE);
            AudioManager.PauseSounds(activeNoteSounds, SoundType.INSTRUMENT);
            _gameUI.HidePauseButton();
            PauseMenu.Instance.Open();
        }
        void OnGameResumed()
        {
            AudioManager.UnPauseSound(droneNote, SoundType.DRONE);
            AudioManager.UnPuaseSounds(activeNoteSounds, SoundType.INSTRUMENT);
            _gameUI.ShowPauseButton();
        }
        #endregion

        #region HELPERS   

        void PlayInstrumentNote(string note)
        {
            activeNoteSounds.Add(note);
            AudioManager.PlaySound(note, SoundType.INSTRUMENT);
        }
        void StopInstrumentNote(string note)
        {
            activeNoteSounds.Remove(note);
            AudioManager.StopSound(note, SoundType.INSTRUMENT);
        }
        void PlayDroneNoteEffect() => StartCoroutine(DroneNoteEffectRoutine());
        IEnumerator DroneNoteEffectRoutine()
        {
            _gameUI.DisplayDroneText("Drone: " + droneNote);
            AudioManager.PlaySound(droneNote, SoundType.DRONE);
            yield return new WaitForSeconds(0.5f);
            _gameUI.PulseDroneText();
        }
        void StopDroneNoteEffect()
        {
            AudioManager.StopSound(droneNote, SoundType.DRONE);
            _gameUI.StopDronePulse();
        }
        #endregion

        #region UTILITY
        public static void Stop()
        {
            if (_instance == null) return;

            AudioManager.StopAllNoteSounds();
            _instance.StopAllCoroutines();
            _instance._gameUI.StopAllCoroutines();
            //_instance._timer.StopAllCoroutines();
            _instance._scoreSystem.StopAllCoroutines();
            _instance._textSystem.StopAllCoroutines();
        }
        public static void PlayNextLevel()
        {
            GameManager.IncrementLevel();
            Play();
        }

        #endregion
    }
}
