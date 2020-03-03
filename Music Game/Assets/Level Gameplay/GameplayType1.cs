using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using System;

public class GameplayType1 : LevelGameplay
{
    [SerializeField] Sound[] notes = null;
    [SerializeField] GameObject guessButtonsContainer = null;
    [SerializeField] Button choiceButtonPrefab = null;
    [SerializeField] int pointsPerCorrectGuess = 10;
    //[Range(50f, 100f)] [SerializeField] float levelPassPercentage = 75f;

    List<Button> guessButtons = new List<Button>();    
    List<string> unlockedNotesList = new List<string>();
    //List<string> randomizedNotesList = new List<string>();
    int currentNoteIndex;

    #region SETUP
    protected override void Start()
    {
        InitializeNotes(); // will need a way to remove sounds from audio manager
        InitializeGuessButtons();
        base.Start();       
    }
    protected override void SetupLevel()
    {
        base.SetupLevel();
        GameplayUtility.HideButtons(guessButtons, unlockedNotesList);
        GameplayUtility.Timer.EnableTimer();
        GameplayUtility.ScoreSystem.EnableScoreSystem();
        currentNoteIndex = 0;
        unlockedNotesList.Clear();
        unlockedNotesList.Add(notes[0].name);
    }
    protected override void StartGameLoop()
    {
        GameplayUtility.Timer.StartCountdown(3, 0.8f);
    }
    void InitializeNotes()
    {
        foreach (var sound in notes)
        {
            AudioManager.AddSound(sound);
        }
    }
    void InitializeGuessButtons()
    {
        guessButtons.Clear();
        foreach (var sound in notes)
        {
            Button b = Instantiate(choiceButtonPrefab, guessButtonsContainer.transform);
            b.gameObject.SetActive(false);

            TextMeshProUGUI textGUI = b.GetComponentInChildren<TextMeshProUGUI>();
            textGUI.text = sound.name;

            b.onClick.AddListener(delegate { OnGuessButtonPressed(b); });

            guessButtons.Add(b);
        }
    }
    #endregion

    #region GAMEPLAY
    IEnumerator PlayGameLoopRoutine()
    {
        if (currentNoteIndex == 0)
        {
            GameplayUtility.HideButtons(guessButtons, unlockedNotesList);
            GameplayUtility.RandomizeList(unlockedNotesList);

            yield return StartCoroutine(GameplayUtility.DisplayButtonsRoutine(guessButtons, unlockedNotesList.Count, .3f));    
            // I dont want the buttons enabled when displayed, only after the note has been played
        }
        StartCoroutine(PlayCurrentNoteRountine());     
    }  
    void ContinueGameLoop()
    {
        currentNoteIndex++;
        if (IsLevelComplete())
        {
            GameplayUtility.HideButtons(guessButtons, unlockedNotesList);
            LevelCompleteMenu.Instance.SetFinalScore(GameplayUtility.ScoreSystem.PlayerScore);
            MenuManager.Instance.OpenMenu(LevelCompleteMenu.Instance);
        }
        else
        {
            if (currentNoteIndex == unlockedNotesList.Count)
            {
                currentNoteIndex = 0;
                // unlock next note
                unlockedNotesList.Add(notes[unlockedNotesList.Count].name);
            }
            StartCoroutine(PlayGameLoopRoutine());
        }
    }
    
    void OnGuessButtonPressed(Button guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));
    protected override void OnCountdownCompleted()
    {
        base.OnCountdownCompleted();
        StartCoroutine(PlayGameLoopRoutine());
    }
    protected override void OnTimerExpired()
    {
        StartCoroutine(TimerExpiredRoutine());
    }
    #endregion

    #region HELPERS
    IEnumerator PlayCurrentNoteRountine()
    {
        AudioManager.PlaySoundOneShot(unlockedNotesList[currentNoteIndex]);
        yield return new WaitForSeconds(1.5f);
        GameplayUtility.EnableButtons(guessButtons, unlockedNotesList); // new
        GameplayUtility.Timer.StartGuessTimer();
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        GameplayUtility.Timer.StopGuessTimer();
        GameplayUtility.EnableButtons(guessButtons, unlockedNotesList, false);

        bool isGuessCorrect = unlockedNotesList[currentNoteIndex] == guessButton.GetComponentInChildren<TextMeshProUGUI>().text;
        if (isGuessCorrect)
        {
            GameplayUtility.ChangeButtonColor(guessButton, Color.green);
            if (correctGuessSound) AudioManager.PlaySoundOneShot(correctGuessSound.name);
            GameplayUtility.ScoreSystem.UpdatePlayerScore(pointsPerCorrectGuess);
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            GameplayUtility.ChangeButtonColor(guessButton, Color.red);
            if (wrongGuessSound) AudioManager.PlaySoundOneShot(wrongGuessSound.name);
            GameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        }

        yield return new WaitForSeconds(1f);

        GameplayUtility.ResetButtonColor(guessButton);
        GameplayUtility.Timer.ResetGuessTimer(timePerGuess);
        GameplayUtility.EnableButtons(guessButtons, unlockedNotesList);

        ContinueGameLoop();
    }
    IEnumerator TimerExpiredRoutine()
    {
        GameplayUtility.EnableButtons(guessButtons, unlockedNotesList, false);
        GameplayUtility.Timer.StopGuessTimer();
        GameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        // play time out effect and sound
        yield return new WaitForSeconds(1.5f);
        GameplayUtility.Timer.ResetGuessTimer(timePerGuess);
        ContinueGameLoop();
    }
    public override bool IsLevelComplete() => currentNoteIndex == notes.Length;
    public override bool IsLevelPassed()
    {
        return false;
    }
    public override void PauseGame()
    {
        GameplayUtility.Timer.StopGuessTimer();
        //AudioManager.PauseSound(GameplayUtility.Timer.TimerSound.name);
    }
    public override void ResumeGame()
    {
        //AudioManager.PauseSound(GameplayUtility.Timer.TimerSound.name, false);
    }

    #endregion

    
}
