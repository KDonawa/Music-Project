using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayType2 : LevelGameplay
{ 
    int currentNumGuessesGiven;

    List<string> answers;
    List<string> activeSounds;

    Coroutine droneNoteEffectRoutine;

    #region SETUP
    protected override void Awake()
    {
        base.Awake();
        answers = new List<string>();
        activeSounds = new List<string>();
    }
    protected override void SetupLevel()
    {
        base.SetupLevel();
        answers.Clear();
        activeSounds.Clear();

        

    }
    #endregion

    #region GAMEPLAY

    protected override void PlayGameLoop() => StartCoroutine(PlayGameLoopRoutine());
    protected override void OnGuessButtonPressed(Button guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));
    void PlayCurrentNotes() => StartCoroutine(PlayCurrentNotesRoutine());
    #endregion

    #region HELPERS
    IEnumerator PlayIntro()
    {
        StartCoroutine(_gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f, false));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < currentNotes.Count; i++)
        {
            Button b = guessButtons[i];
            _gameplayUtility.ChangeButtonColor(b, Color.green);
            string noteToPlay = _gameplayUtility.GetWesternNotation(currentLevel.subLevels[currentSubLevel].notes[i], droneNote);
            AudioManager.Instance.PlaySound(noteToPlay);
            yield return new WaitForSeconds(2.5f);
            _gameplayUtility.ResetButtonColor(b);
            yield return new WaitForSeconds(0.2f);
        }
        _gameplayUtility.HideButtons(guessButtons);
    }
    IEnumerator PlayGameLoopRoutine()
    {
        //yield return StartCoroutine(PlayIntro());
        yield return null;
        
        PlayCurrentNotes();
    }
    void PlayNote(string note)
    {
        activeSounds.Add(note);
        //AudioManager.Instance.PlaySound(note);
    }

    void StopNote(string note)
    {
        activeSounds.Remove(note);
        //AudioManager.Instance.StopSound(note);
    }
    void StartDroneNoteEffect()
    {
        droneNoteEffectRoutine = StartCoroutine(_gameplayUtility.GrowAndShrinkTextRoutine(_gameUI.DroneText, 1.2f, 0.5f));
    }
    void StopDroneNoteEffect()
    {
        if (droneNoteEffectRoutine != null) StopCoroutine(droneNoteEffectRoutine);
        _gameUI.ResetDroneText();
    }
    IEnumerator PlayCurrentNotesRoutine()
    {
        currentNumGuessesGiven = 0;
        answers.Clear();

        // drone effects
        _gameUI.DisplayDroneText("Drone: " + _gameplayUtility.GetDroneNoteFormatted(droneNote));        
        PlayNote(droneNote);
        yield return new WaitForSeconds(0.5f);
        StartDroneNoteEffect();
        yield return new WaitForSeconds(4f);

        // game messages
        _gameUI.DisplayGameText("Playing Notes");
        
        // play each note consecutively
        for (int i = 0; i < numNotesPlayedPerGuess; i++)
        {
            string note = currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)];
            answers.Add(note);
            PlayNote(_gameplayUtility.GetWesternNotation(note, droneNote));
            _gameUI.DisplayDebugText(_gameplayUtility.GetIndianNotationFormatted(note)); // testing

           yield return new WaitForSeconds(2.5f);
            StopNote(_gameplayUtility.GetWesternNotation(note, droneNote));            
        }

        StopNote(droneNote);
        StopDroneNoteEffect();
        _gameUI.HideGameText();
        _gameUI.HideDroneText();
        _gameUI.HideDebugText();        

        // display guess options
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(_gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .3f));              

        //start timer
        _gameplayUtility.Timer.StartGuessTimer();
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        _gameplayUtility.DisableButtons(guessButtons);
        currentNumGuessesGiven++;

        if(currentNumGuessesGiven == numNotesPlayedPerGuess)
        {
            _gameplayUtility.Timer.StopGuessTimer();
            _gameplayUtility.EnableButtons(guessButtons, false);
        }
                
        bool isGuessCorrect = answers[currentNumGuessesGiven - 1] == guessButton.GetComponent<ChoiceButton>().NoteName;
        if (isGuessCorrect)
        {
            _gameplayUtility.ChangeButtonColor(guessButton, Color.green);
            AudioManager.Instance.PlaySound("correct guess");            
        }
        else
        {
            _gameplayUtility.ChangeButtonColor(guessButton, Color.red);
            AudioManager.Instance.PlaySound("wrong guess");            
        }
        yield return new WaitForSeconds(0.5f);
        _gameplayUtility.ResetButtonColor(guessButton);

        _gameplayUtility.ScoreSystem.UpdateGuessAccuracy(isGuessCorrect);
        _gameplayUtility.TextSystem.DisplayGuessFeedback(isGuessCorrect);
        

        if (currentNumGuessesGiven == numNotesPlayedPerGuess)
        {
            yield return new WaitForSeconds(1.5f);
            _gameplayUtility.ResetButtonColor(guessButton);
            _gameplayUtility.HideButtons(guessButtons);
            _gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
            yield return new WaitForSeconds(1f);

            ContinueGameLoop();
        }
        else
        {
            _gameplayUtility.EnableButtons(guessButtons);
        }
    }
    #endregion

    #region UTILITY

    public override void PauseGame()
    {
        foreach (var sound in activeSounds)
        {
            AudioManager.Instance.PauseSound(sound);
        }
    }
    public override void ResumeGame()
    {
        foreach (var sound in activeSounds)
        {
            AudioManager.Instance.UnPauseSound(sound);
        }
    }
    #endregion

}
