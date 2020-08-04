using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
    TODO:    
    -fix level complete menu
    -stage complete
    -next level only available if you pass the level: 80%
    -make transitions where the buttons fly off the screen
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
        AudioManager.StopAllGamplaySounds();        

        currentLevel = GameManager.Instance.GetCurrentLevel();
        currentSubLevel = 0;
        numNotesPlayedPerGuess = currentLevel.numNotesToGuess;

        _gameUI.Inititialize();
        _gameplayUtility.Timer.Initialize(timeToGuessPerNote * numNotesPlayedPerGuess);
        _gameplayUtility.ScoreSystem.Initialize(numNotesPlayedPerGuess * currentLevel.subLevels.Length);
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
            Button b = _gameUI.InstantiateGuessButton(note, _gameplayUtility.GetIndianNotationFormatted(note));
            b.onClick.AddListener(() => OnGuessButtonPressed(b.GetComponent<GuessButton>()));
            guessButtons.Add(b);
        }
    }
    #endregion

    #region GAMEPLAY

    private void Play()
    {
        StartCoroutine(PlayWhenLoadCompleteRoutine());
    }

    protected abstract void OnTimerExpired();
    protected abstract void PlayGameLoop();
    protected void ContinueGameLoop()
    {
        currentSubLevel++;
        // may need to make this a routine eventually
        if (IsLevelComplete())
        {
            //end level routine
            _gameplayUtility.HideButtons(guessButtons);
            _gameplayUtility.Timer.ResetGuessTimer();
            LevelCompleteMenu.SetFinalScore(_gameplayUtility.ScoreSystem.GetPlayerScorePercentage());
            MenuManager.Instance.OpenMenu(LevelCompleteMenu.Instance);
        }
        else
        {
            _gameplayUtility.Timer.ResetGuessTimer();
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