using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public abstract class LevelGameplay : MonoBehaviour
{
    //[SerializeField] bool showAllPossibleGuesses = false;
    [Range(0, 5f)] [SerializeField] protected float timeToGuessPerNote = 3f;
    protected float timePerGuess;

    [SerializeField] protected LevelGameplayUtility gameplayUtility;
    //protected int currentLevelIndex;
    [SerializeField] protected TextMeshProUGUI levelText = null;
    [SerializeField] protected TextMeshProUGUI droneText = null;
    [SerializeField] protected GameObject guessButtonsContainer = null;
    [SerializeField] protected Button choiceButtonPrefab = null;

    protected Level currentLevel;
    protected List<Button> guessButtons;
    protected List<string> currentNotes;
    protected string droneNote;
    protected int numNotesPlayedPerGuess;
    protected int currentSubLevel;
    //StringBuilder sb;

    #region SETUP
    protected virtual void Awake()
    {
        gameplayUtility = Instantiate(gameplayUtility);
        guessButtons = new List<Button>();
        currentNotes = new List<string>();
        droneNote = string.Empty;
        //sb = new StringBuilder(30);
    }
    protected virtual void OnEnable()
    {
        
    }
    protected virtual void Start()
    {
        gameplayUtility.Timer.TimerExpiredEvent += OnTimerExpired;
        gameplayUtility.Timer.CountdownCompletedEvent += OnCountdownCompleted;
        PlayLevel();
    }
    protected virtual void OnDisable()
    {
        gameplayUtility.Timer.TimerExpiredEvent -= OnTimerExpired;
        gameplayUtility.Timer.CountdownCompletedEvent -= OnCountdownCompleted;
    }    
    protected virtual void SetupLevel()
    {
        StopAllCoroutines();
        MenuManager.Instance.ClearMenuHistory();
        AudioManager.Instance.StopAllSounds();        
        
        levelText.gameObject.SetActive(false);
        droneText.gameObject.SetActive(false);

        currentLevel = GameManager.Instance.GetCurrentLevel();

        currentSubLevel = 0;
        numNotesPlayedPerGuess = currentLevel.numNotesToGuess;
        timePerGuess = timeToGuessPerNote * numNotesPlayedPerGuess;

        gameplayUtility.Timer.Initialize(timePerGuess);
        gameplayUtility.ScoreSystem.Initialize();
        // init text system

        //GameMenu.Open();

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
            Button b = Instantiate(choiceButtonPrefab, guessButtonsContainer.transform);
            b.gameObject.SetActive(false);

            b.GetComponent<ChoiceButton>().Initialize(note);
            b.GetComponentInChildren<TextMeshProUGUI>().text = gameplayUtility.GetIndianNotationAndFormat(note);

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
            gameplayUtility.HideButtons(guessButtons);
            LevelCompleteMenu.Instance.SetFinalScore(gameplayUtility.ScoreSystem.GetPlayerScorePercentage());
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
        levelText.text = "Level " + GameManager.Instance.currentLevel;
        levelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        levelText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);

        GameMenu.Open();
        gameplayUtility.Timer.EnableTimer();
        //GameplayUtility.ScoreSystem.EnableScoreSystem();
        // enable text system
        yield return new WaitForSeconds(0.2f);

        gameplayUtility.Timer.StartCountdown(1, 0.8f);
    }
    IEnumerator TimerExpiredRoutine()
    {
        gameplayUtility.EnableButtons(guessButtons, false);
        gameplayUtility.Timer.StopGuessTimer();
        gameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        // play time out effect and sound
        yield return new WaitForSeconds(1.5f);
        gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
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