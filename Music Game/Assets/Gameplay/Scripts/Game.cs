using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    TODO:    
    -label level select menu with stage name
    -improve lvl complete: spawn text, then score, etc in intervals
    -instrument select screen
    -locked level and stages icons
    -stars earned for levels will show up
    -make transitions where the buttons fly off the screen
*/
[RequireComponent(typeof(GameUtility))]
public class Game : MonoBehaviour
{
    static Game _instance;

    [Header("Prefabs")]
    [SerializeField] GameUI gameUI = null;

    [Header("Variables")]
    [Range(0, 5)] [SerializeField] int countdownTime = 0;
    [Range(0, 10f)] [SerializeField] float timeToGuessPerNote = 5f;
    [Range(0, 100)] [SerializeField] int levelPassPercentage = 80;
    [SerializeField] bool playIntro = true;

    [Header("Systems")]
    [SerializeField] Timer timerPrefab = null;
    [SerializeField] ScoreSystem scoreSystemPrefab = null;
    [SerializeField] TextSystem textSystemPrefab = null;

    // refs
    GameUI _gameUI;
    GameUtility _utility;
    Timer _timer;
    ScoreSystem _scoreSystem;
    TextSystem _textSystem;

    // variables
    Level currentLevel;
    List<Button> guessButtons;
    List<string> currentNotes;
    string droneNote;
    int numGuessesPerRound;
    int currentSubLevelIndex;
    int guessCount;
    List<string> answers;
    List<string> activeNoteSounds;

    public static event System.Action<int, bool> LevelCompleteEvent;

    #region SETUP
    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);

        _utility = GetComponent<GameUtility>();

        _gameUI = Instantiate(gameUI);
        _timer = Instantiate(timerPrefab);
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
        Timer.TimerExpiredEvent += OnTimerExpired;
        GameManager.GamePausedEvent += OnGamePaused;
        GameManager.GameUnPausedEvent += OnGameResumed;

        Play();
    }
    
    private void OnDestroy()
    {
        if (_instance == this) _instance = null;

        GameManager.GamePausedEvent -= OnGamePaused;
        GameManager.GameUnPausedEvent -= OnGameResumed;
        GuessButton.ButtonPressedEvent -= OnGuessButtonPressed;
        GuessButton.GuessCheckedEvent -= OnGuessButtonChecked;
        Timer.TimerExpiredEvent -= OnTimerExpired;
    }
   
    bool InitializeLevel()
    {      
        MenuManagerUpdated.CloseAllMenus();
        AudioManager.StopAllNoteSounds();
        _utility.HideButtons(guessButtons);

        currentLevel = GameManager.Instance.GetCurrentLevel();
        if (currentLevel == null) return false; // debug.logwarning

        currentSubLevelIndex = 0;
        numGuessesPerRound = currentLevel.numNotesToGuess;

        _gameUI.Inititialize();
        _timer.Initialize(timeToGuessPerNote * numGuessesPerRound);
        _scoreSystem.Initialize(numGuessesPerRound * currentLevel.subLevels.Length);
        _textSystem.Initialize();

        return true;
    }
    void InitializeNotes()
    {
        droneNote = GameManager.Instance.DroneNote;
        currentNotes.Clear();
        foreach (var note in currentLevel.subLevels[currentSubLevelIndex].notes) currentNotes.Add(note);
    }
    void InitializeGuessButtons()
    {
        foreach (var b in guessButtons) Destroy(b.gameObject);
        guessButtons.Clear();
        foreach (var note in currentNotes) guessButtons.Add(_gameUI.InitGuessButton(note, _utility.GetNoteFormatted(note)));
    }
    #endregion

    #region GAMEPLAY

    public static void Play()
    {
        if(_instance.InitializeLevel()) _instance.StartCoroutine(_instance.StartLevelRoutine());
    }
    IEnumerator StartLevelRoutine()
    {
        while (!SceneTransitions.sceneLoadingComplete) yield return null; //wait until scene is loaded

        yield return StartCoroutine(_gameUI.DisplayCurrentLevelRoutine());
        yield return new WaitForSeconds(0.5f);
        _gameUI.StartCountdown(countdownTime, 0.8f, PlayGameLoop);
    }
    void PlayGameLoop()
    {      
        InitializeNotes();
        InitializeGuessButtons();
        _timer.ResetGuessTimer();
        guessCount = 0;
        answers.Clear();
        activeNoteSounds.Clear();
        StartCoroutine(PlayGameLoopRoutine());
    }
    IEnumerator PlayGameLoopRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // Play Intro
        if (playIntro)
        {
            for (int i = 0; i < currentNotes.Count; i++)
            {
                Button b = guessButtons[i];
                _utility.LoadButton(b, false, false);
                yield return new WaitForSeconds(0.5f);

                UIAnimator.FlashButtonColor(b, Color.magenta, 2f);
                AudioManager.PlaySound(_utility.GetWesternNotation(currentNotes[i], droneNote), SoundType.INSTRUMENT);

                yield return new WaitForSeconds(2f);
            }
            _utility.HideButtons(guessButtons);
        }

        yield return new WaitForSeconds(0.5f);

        // Play Notes
        StartCoroutine(PlayNotesRoutine());
    }
    IEnumerator PlayNotesRoutine()
    {
        // drone effects
        _gameUI.DisplayDroneText("Drone: " + droneNote); 
        yield return new WaitForSeconds(1f);
        PlayDroneNoteEffect();
        //yield return new WaitForSeconds(4f);

        // game messages
        _gameUI.DisplayGameText("Playing Notes");

        // play each note consecutively
        for (int i = 0; i < numGuessesPerRound; i++)
        {
            string note = currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)];
            answers.Add(note);
            PlayInstrumentNote(_utility.GetWesternNotation(note, droneNote));
            _gameUI.DisplayDebugText(_utility.GetNoteFormatted(note)); // testing

            yield return new WaitForSeconds(2.5f);
            StopInstrumentNote(_utility.GetWesternNotation(note, droneNote));
        }

        StopDroneNoteEffect();
        _gameUI.HideGameText();
        _gameUI.HideDroneText();
        //_gameUI.HideDebugText();

        // display guess options
        yield return StartCoroutine(_utility.LoadButtonsRoutine(guessButtons, 0.2f, false));
        _utility.EnableButtons(guessButtons);
        //start timer
        _timer.StartGuessTimer();
    }
    void ContinueGameLoop()
    {
        _gameUI.HideDebugText();
        _timer.StopGuessTimer();
        currentSubLevelIndex++;

        if (IsLevelComplete()) LevelCompleteEvent?.Invoke(_scoreSystem.FinalScorePercentage(), IsLevelPassed());
        else PlayGameLoop();
    }
    public bool IsLevelComplete() => currentSubLevelIndex == currentLevel.subLevels.Length;
    public bool IsLevelPassed() => _scoreSystem.FinalScorePercentage() >= levelPassPercentage;
    #endregion

    #region EVENTS
    void OnTimerExpired() => StartCoroutine(TimerExpiredRoutine());
    IEnumerator TimerExpiredRoutine()
    {
        _utility.DisableButtons(guessButtons);

        AudioManager.PlaySound(AudioManager.timerExpired, SoundType.UI);

        yield return new WaitForSeconds(1f);
        _utility.HideButtons(guessButtons);
        yield return new WaitForSeconds(1f);
        ContinueGameLoop();
    }
    void OnGuessButtonPressed(GuessButton guessButton)
    {
        _timer.StopGuessTimer();
        _utility.DisableButtons(guessButtons);
        GuessButton.correctGuess = answers[guessCount++];
    }
    void OnGuessButtonChecked()
    {
        if (guessCount == numGuessesPerRound)
        {
            ContinueGameLoop();
        }
        else
        {
            _utility.EnableButtons(guessButtons);
            _timer.StartGuessTimer();
        }
    }
    void OnGamePaused()
    {
        AudioManager.PauseSound(droneNote, SoundType.DRONE);
        AudioManager.PauseSounds(activeNoteSounds, SoundType.INSTRUMENT);
        PauseMenu.Instance.Open();
    }
    void OnGameResumed()
    {
        AudioManager.UnPauseSound(droneNote, SoundType.DRONE);
        AudioManager.UnPuaseSounds(activeNoteSounds, SoundType.INSTRUMENT);
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
    void PlayDroneNoteEffect()
    {
        AudioManager.PlaySound(droneNote, SoundType.DRONE);
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
        _instance._utility.StopAllCoroutines();
        _instance._timer.StopAllCoroutines();
        _instance._scoreSystem.StopAllCoroutines();
        _instance._textSystem.StopAllCoroutines();     
    }
    public static void PlayNextLevel()
    {
        GameManager.Instance.IncrementLevel();
        Play();
    }
    
    
    #endregion
}