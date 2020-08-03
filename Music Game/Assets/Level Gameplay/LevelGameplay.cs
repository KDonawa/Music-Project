using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
    TODO:
    -next level only available if you pass the level: 80%
    -have graphic and sound effect for screen transitions
        -stage select will rotate the buttons and screen will move to left in transition to level select
        -or button selected will light up and others will disappear
    -button press effects
*/
public abstract class LevelGameplay : MonoBehaviour
{
    [Range(0, 5f)] [SerializeField] protected float timeToGuessPerNote = 3f;
    

    protected GameUI _gameUI;
    protected LevelGameplayUtility _gameplayUtility;
    protected Level currentLevel;
    protected List<Button> guessButtons;
    protected List<string> currentNotes;
    protected string droneNote;
    protected int numNotesPlayedPerGuess;
    protected int currentSubLevel;

    #region SETUP
    protected virtual void Awake()
    {
        _gameUI = Instantiate(GameManager.Instance.GameUI);
        _gameplayUtility = Instantiate(GameManager.Instance.GameplayUtility);
        guessButtons = new List<Button>();
        currentNotes = new List<string>();
        droneNote = string.Empty;
    }
    protected virtual void Start()
    {
        Timer.TimerExpiredEvent += OnTimerExpired;
        Play();
    }
    
    private void OnDestroy()
    {
        Timer.TimerExpiredEvent -= OnTimerExpired;
    }
  
    protected virtual void InitializeLevel()
    {
        StopAllCoroutines();
        MenuManager.Instance.ClearMenuHistory();
        AudioManager.Instance.StopAllSounds();        

        currentLevel = GameManager.Instance.GetCurrentLevel();
        currentSubLevel = 0;
        numNotesPlayedPerGuess = currentLevel.numNotesToGuess;

        _gameUI.Inititialize();
        _gameplayUtility.Timer.Initialize(timeToGuessPerNote * numNotesPlayedPerGuess);
        _gameplayUtility.ScoreSystem.Initialize();
        _gameplayUtility.TextSystem.Initialize();

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

        foreach (var note in currentNotes)
        {
            Button b = _gameUI.InstantiateGuessButton();
            b.gameObject.SetActive(false);

            b.GetComponent<GuessButton>().Initialize(note);
            b.GetComponentInChildren<TextMeshProUGUI>().text = _gameplayUtility.GetIndianNotationFormatted(note);

            b.onClick.AddListener(delegate { OnGuessButtonPressed(b.GetComponent<GuessButton>()); });
            guessButtons.Add(b);
        }
    }
    #endregion

    #region GAMEPLAY

    private void Play()
    {
        StartCoroutine(PlayWhenLoadCompleteRoutine());
    }

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
    protected abstract void OnGuessButtonPressed(GuessButton guessButton);

    #endregion

    #region HELPERS
    IEnumerator PlayWhenLoadCompleteRoutine()
    {
        while (!SceneTransitions.sceneLoadingComplete)
        {
            yield return null;
        }
        InitializeLevel();
        StartCoroutine(StartLevelRoutine());
    }
    IEnumerator StartLevelRoutine()
    {
        yield return StartCoroutine(_gameUI.DisplayCurrentLevelRoutine());

        GameMenu.Open();
        _gameplayUtility.Timer.DisplayTimer();
        _gameplayUtility.ScoreSystem.DisplayScoreSystem(false);

        yield return new WaitForSeconds(0.2f);

        _gameplayUtility.Timer.StartCountdown(0, 0.8f, PlayGameLoop);
    }
    IEnumerator TimerExpiredRoutine()
    {
        _gameplayUtility.EnableButtons(guessButtons, false);
        _gameplayUtility.Timer.StopGuessTimer();
        _gameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        // play time out effect and sound
        yield return new WaitForSeconds(1.5f);
        _gameplayUtility.Timer.ResetGuessTimer();
        ContinueGameLoop();
    }
    
    #endregion

    #region UTILITY

    public void RestartLevel() => Play();
    public void PlayNextLevel()
    {
        GameManager.Instance.IncrementLevel();
        Play();
    }       
    public abstract void PauseGame();
    public abstract void ResumeGame();
    public virtual bool IsLevelComplete() => currentSubLevel == currentLevel.subLevels.Length;
    public virtual bool IsLevelPassed() => true;
    #endregion
}