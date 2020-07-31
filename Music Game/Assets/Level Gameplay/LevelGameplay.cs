using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
    TODO:
    -update win screen/pause menu
    -have graphic and sound effect for screen transitions
*/
public abstract class LevelGameplay : MonoBehaviour
{
    //[SerializeField] bool showAllPossibleGuesses = false;
    [Range(0, 5f)] [SerializeField] protected float timeToGuessPerNote = 3f;
    

    protected GameUI _gameUI;
    protected LevelGameplayUtility _gameplayUtility;
    protected Level currentLevel;
    protected List<Button> guessButtons;
    protected List<string> currentNotes;
    protected string droneNote;
    protected int numNotesPlayedPerGuess;
    protected int currentSubLevel;
    protected float timePerGuess;

    #region SETUP
    protected virtual void Awake()
    {
        _gameUI = Instantiate(GameManager.Instance.GameUI);
        _gameplayUtility = Instantiate(GameManager.Instance.GameplayUtility);
        guessButtons = new List<Button>();
        currentNotes = new List<string>();
        droneNote = string.Empty;
    }
    protected virtual void OnEnable()
    {
        
    }
    protected virtual void Start()
    {
        _gameplayUtility.Timer.TimerExpiredEvent += OnTimerExpired;
        _gameplayUtility.Timer.CountdownCompletedEvent += OnCountdownCompleted;
        PlayLevel();
    }
    protected virtual void OnDisable()
    {
        _gameplayUtility.Timer.TimerExpiredEvent -= OnTimerExpired;
        _gameplayUtility.Timer.CountdownCompletedEvent -= OnCountdownCompleted;
    }    
    protected virtual void SetupLevel()
    {
        StopAllCoroutines();
        MenuManager.Instance.ClearMenuHistory();
        AudioManager.Instance.StopAllSounds();

        _gameUI.Inititialize();

        currentLevel = GameManager.Instance.GetCurrentLevel();

        currentSubLevel = 0;
        numNotesPlayedPerGuess = currentLevel.numNotesToGuess;
        timePerGuess = timeToGuessPerNote * numNotesPlayedPerGuess;

        _gameplayUtility.Timer.Initialize(timePerGuess);
        _gameplayUtility.ScoreSystem.Initialize();
        // init text system

        InitializeNotes();
        InitializeGuessOptions();
    }
    void InitializeNotes()
    {
        droneNote = currentLevel.droneNote.name;
        currentNotes.Clear();

        foreach (var note in currentLevel.subLevels[currentSubLevel].notes)
        {
            currentNotes.Add(note);
        }
    }
    void InitializeGuessOptions()
    {
        foreach (var b in guessButtons) Destroy(b.gameObject);
        guessButtons.Clear();

        //string[] guessesToDisplay = null;
        //if (showAllPossibleGuesses) guessesToDisplay = currentLevel.subLevels[currentLevel.subLevels.Length - 1].notes;
        //else guessesToDisplay = currentLevel.subLevels[0].notes;
        //guessesToDisplay = currentLevel.subLevels[currentSubLevel].notes;
        foreach (var note in currentNotes)
        {
            Button b = _gameUI.InstantiateGuessButton();
            b.gameObject.SetActive(false);

            b.GetComponent<ChoiceButton>().Initialize(note);
            b.GetComponentInChildren<TextMeshProUGUI>().text = _gameplayUtility.GetIndianNotationFormatted(note);

            b.onClick.AddListener(delegate { OnGuessButtonPressed(b); });
            guessButtons.Add(b);
        }
    }
    #endregion

    #region GAMEPLAY

    private void PlayLevel()
    {
        SetupLevel();
        StartCoroutine(StartLevelRoutine());
    }
    private void OnCountdownCompleted() => PlayGameLoop();
    private void OnTimerExpired() => StartCoroutine(TimerExpiredRoutine());
    protected abstract void PlayGameLoop();
    protected void ContinueGameLoop()
    {
        currentSubLevel++;
        // may need to make this a routine eventually
        if (IsLevelComplete())
        {
            //end level routine
            _gameplayUtility.HideButtons(guessButtons);
            LevelCompleteMenu.Instance.SetFinalScore(_gameplayUtility.ScoreSystem.GetPlayerScorePercentage());
            MenuManager.Instance.OpenMenu(LevelCompleteMenu.Instance);
        }
        else
        {
            InitializeNotes();
            InitializeGuessOptions();
            PlayGameLoop();
        }
    }
    protected abstract void OnGuessButtonPressed(Button guessButton);
    
    #endregion

    #region HELPERS

    IEnumerator StartLevelRoutine()
    {
        yield return StartCoroutine(_gameUI.DisplayCurrentLevelRoutine());

        GameMenu.Open();
        _gameplayUtility.Timer.EnableTimer();
        //GameplayUtility.ScoreSystem.EnableScoreSystem();
        // enable text system
        yield return new WaitForSeconds(0.2f);

        _gameplayUtility.Timer.StartCountdown(0, 0.8f);
    }
    IEnumerator TimerExpiredRoutine()
    {
        _gameplayUtility.EnableButtons(guessButtons, false);
        _gameplayUtility.Timer.StopGuessTimer();
        _gameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        // play time out effect and sound
        yield return new WaitForSeconds(1.5f);
        _gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
        ContinueGameLoop();
    }
    
    #endregion

    #region UTILITY

    public void RestartLevel() => PlayLevel();
    public void PlayNextLevel()
    {
        GameManager.Instance.IncrementLevel();
        PlayLevel();
    }       
    public abstract void PauseGame();
    public abstract void ResumeGame();
    public virtual bool IsLevelComplete() => currentSubLevel == currentLevel.subLevels.Length;
    public virtual bool IsLevelPassed() => true;
    #endregion
}