using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
    TODO:    
    -fix menu system
    -button press effect also changes text color
    -drone select screen
    -instrument select screen
    -stage complete menu
    -make transitions where the buttons fly off the screen
*/
public class Game : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameUI gameUI = null;
    [SerializeField] LevelGameplayUtility gameplayUtility = null;

    [Header("Variables")]
    [Range(0, 5)] [SerializeField] int countdownTime = 0;
    [Range(0, 10f)] [SerializeField] float timeToGuessPerNote = 5f;
    [Range(0, 100)] [SerializeField] int levelPassPercentage = 80;
    [SerializeField] bool playIntro = true;


    GameUI _gameUI;
    LevelGameplayUtility _gameplayUtility;
    Level currentLevel;
    List<Button> guessButtons;
    List<string> currentNotes;
    string droneNote;
    int numNotesPlayedPerGuess;
    int currentSubLevel;
    int guessCount;
    List<string> answers;
    List<string> activeNoteSounds;

    //public static event System.Action<bool> LevelCompleteEvent;

    #region SETUP
    private void Awake()
    {
        _gameUI = Instantiate(gameUI);
        _gameplayUtility = Instantiate(gameplayUtility);
        guessButtons = new List<Button>();
        currentNotes = new List<string>();
        droneNote = string.Empty;
        answers = new List<string>();
        activeNoteSounds = new List<string>();
    }
    private void Start()
    {
        GuessButton.GuessEvent += GuessButtonPressed;
        Timer.TimerExpiredEvent += OnTimerExpired;
        Play();
    }
    
    private void OnDestroy()
    {
        GuessButton.GuessEvent -= GuessButtonPressed;
        Timer.TimerExpiredEvent -= OnTimerExpired;
    }
  
    void InitializeLevel()
    {
        MenuManager.Instance.ClearMenuHistory();
        AudioManager.StopAllGamplaySounds();        

        currentLevel = GameManager.Instance.GetCurrentLevel();
        currentSubLevel = 0;
        numNotesPlayedPerGuess = currentLevel.numNotesToGuess;

        _gameUI.Inititialize();
        _gameplayUtility.Timer.Initialize(timeToGuessPerNote * numNotesPlayedPerGuess);
        _gameplayUtility.ScoreSystem.Initialize(numNotesPlayedPerGuess * currentLevel.subLevels.Length);
        _gameplayUtility.TextSystem.Initialize();
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
            guessButtons.Add(_gameUI.InitGuessButton(note, _gameplayUtility.GetINFormatted(note)));
        }
    }
    #endregion

    #region GAMEPLAY

    void Play()
    {
        StopAllCoroutines();
        StartCoroutine(StartLevelRoutine());
    }
    void PlayGameLoop()
    {
        InitializeNotes();
        InitializeGuessOptions();
        guessCount = 0;
        answers.Clear();
        activeNoteSounds.Clear();
        StartCoroutine(PlayGameLoopRoutine());
    }
        
    void ContinueGameLoop()
    {
        _gameplayUtility.HideButtons(guessButtons);
        _gameplayUtility.Timer.ResetGuessTimer();
        currentSubLevel++;
        // may need to make this a routine eventually
        if (IsLevelComplete())
        {
            LevelCompleteMenu.DisplayMenu(IsLevelPassed());
        }
        else
        {
            PlayGameLoop();
        }
    }
    void OnTimerExpired() => StartCoroutine(TimerExpiredRoutine());    
    void GuessButtonPressed(GuessButton guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));

    #endregion

    #region HELPERS
    IEnumerator StartLevelRoutine()
    {
        //wait until scene is loaded
        while (!SceneTransitions.sceneLoadingComplete) yield return null;

        InitializeLevel();
        yield return StartCoroutine(_gameUI.DisplayCurrentLevelRoutine());
        
        GameMenu.Open();        
        _gameplayUtility.Timer.DisplayTimer();

        yield return new WaitForSeconds(0.2f);

        _gameplayUtility.Timer.StartCountdown(countdownTime, 0.8f, PlayGameLoop);
    }
    IEnumerator PlayGameLoopRoutine()
    {
        // Play Intro
        yield return new WaitForSeconds(0.5f);
        if (playIntro)
        {
            for (int i = 0; i < currentNotes.Count; i++)
            {
                Button b = guessButtons[i];
                _gameplayUtility.LoadButton(b, false);
                UIAnimator.SetColor(b.GetComponent<Image>(), Color.green);
                string noteToPlay = _gameplayUtility.GetWesternNotation(currentLevel.subLevels[currentSubLevel].notes[i], droneNote);
                AudioManager.PlaySound(noteToPlay, SoundType.HARMONIUM);
                yield return new WaitForSeconds(2.5f);
                UIAnimator.SetColor(b.GetComponent<Image>(), Color.black);
                yield return new WaitForSeconds(0.2f);
            }
            _gameplayUtility.HideButtons(guessButtons);
        }        

        // Play Notes
        StartCoroutine(PlayNotesRoutine());
    }    
    IEnumerator PlayNotesRoutine()
    {
        // drone effects
        _gameUI.DisplayDroneText("Drone: " + _gameplayUtility.GetDroneNoteFormatted(droneNote));
        yield return new WaitForSeconds(0.5f);
        PlayDroneNoteEffect();
        //yield return new WaitForSeconds(4f);

        // game messages
        _gameUI.DisplayGameText("Playing Notes");

        // play each note consecutively
        for (int i = 0; i < numNotesPlayedPerGuess; i++)
        {
            string note = currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)];
            answers.Add(note);
            PlayNote(_gameplayUtility.GetWesternNotation(note, droneNote));
            _gameUI.DisplayDebugText(_gameplayUtility.GetINFormatted(note)); // testing

            yield return new WaitForSeconds(2.5f);
            //yield return new WaitForSeconds(1f);
            StopNote(_gameplayUtility.GetWesternNotation(note, droneNote));
        }

        StopDroneNoteEffect();
        _gameUI.HideGameText();
        _gameUI.HideDroneText();
        _gameUI.HideDebugText();

        // display guess options
        yield return StartCoroutine(_gameplayUtility.LoadButtonsRoutine(guessButtons, 0.3f));

        //start timer
        _gameplayUtility.Timer.StartGuessTimer();
    }
    IEnumerator GuessButtonPressedRoutine(GuessButton guessButton)
    {
        _gameplayUtility.Timer.StopGuessTimer();
        _gameplayUtility.DisableButtons(guessButtons);
        guessCount++;

        // check guess
        guessButton.ProcessGuess(answers[guessCount - 1]);

        yield return new WaitForSeconds(1f);

        // we have no more notes to guess
        if (guessCount == numNotesPlayedPerGuess)
        {
            //_gameplayUtility.HideButtons(guessButtons);
            //yield return new WaitForSeconds(1f);
            ContinueGameLoop();
        }
        else
        {
            _gameplayUtility.EnableButtons(guessButtons);
            _gameplayUtility.Timer.StartGuessTimer();
        }
    }    
    IEnumerator TimerExpiredRoutine()
    {
        _gameplayUtility.DisableButtons(guessButtons);

        AudioManager.PlaySound(AudioManager.timerExpired, SoundType.UI);

        yield return new WaitForSeconds(1f);
        _gameplayUtility.HideButtons(guessButtons);
        yield return new WaitForSeconds(1f);
        ContinueGameLoop();
    }
    
    void PlayNote(string note)
    {
        activeNoteSounds.Add(note);
        //AudioManager.PlaySound(note, SoundType.HARMONIUM);
    }
    void StopNote(string note)
    {
        activeNoteSounds.Remove(note);
        //AudioManager.StopSound(note, SoundType.HARMONIUM);
    }
    void PlayDroneNoteEffect()
    {
        //AudioManager.PlaySound(droneNote, SoundType.DRONE);
        _gameUI.PulseDroneText();
    }
    void StopDroneNoteEffect()
    {
        //AudioManager.StopSound(droneNote, SoundType.DRONE);
        _gameUI.StopDronePulse();
    }
    #endregion

    #region UTILITY
    void IncrementGuessCount() => guessCount++;
    protected void ResetGuessCount() => guessCount = 0;
    public void RestartLevel() => Play();
    public void PlayNextLevel()
    {
        GameManager.Instance.IncrementLevel();
        Play();
    }
    public void PauseGame()
    {
        AudioManager.PauseSound(droneNote, SoundType.DRONE);

        foreach (var sound in activeNoteSounds)
        {
            AudioManager.PauseSound(sound, SoundType.HARMONIUM);
        }
    }
    public void ResumeGame()
    {
        AudioManager.UnPauseSound(droneNote, SoundType.DRONE);

        foreach (var sound in activeNoteSounds)
        {
            AudioManager.UnPauseSound(sound, SoundType.HARMONIUM);
        }
    }
    public bool IsLevelComplete() => currentSubLevel == currentLevel.subLevels.Length;
    public bool IsLevelPassed() => ScoreSystem.GetPlayerScorePercentageAsInt() >= levelPassPercentage;
    #endregion
}