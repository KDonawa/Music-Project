using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayType2 : LevelGameplay
{ 
    int currentNumGuessesGiven;

    List<string> answers;
    List<string> activeNoteSounds;

    #region SETUP
    protected override void Awake()
    {
        base.Awake();
        answers = new List<string>();
        activeNoteSounds = new List<string>();
    }
    protected override void InitializeLevel()
    {
        base.InitializeLevel();
        answers.Clear();
        activeNoteSounds.Clear();     
    }
    #endregion

    #region GAMEPLAY

    protected override void PlayGameLoop() => StartCoroutine(PlayGameLoopRoutine());
    IEnumerator PlayGameLoopRoutine()
    {
        //yield return StartCoroutine(PlayIntro());
        yield return null;

        StartCoroutine(PlayNotesRoutine());
    }
    protected override void OnGuessButtonPressed(GuessButton guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));
    IEnumerator GuessButtonPressedRoutine(GuessButton guessButton)
    {
        _gameplayUtility.Timer.StopGuessTimer();
        _gameplayUtility.DisableButtons(guessButtons);
        currentNumGuessesGiven++;

        // check guess
        guessButton.CheckGuess(answers[currentNumGuessesGiven - 1], _gameplayUtility.ScoreSystem.IncrementNumCorrectGuesses);

        // we have no more notes to guess
        if (currentNumGuessesGiven == numNotesPlayedPerGuess)
        {
            yield return new WaitForSeconds(1f);
            _gameplayUtility.HideButtons(guessButtons);
            yield return new WaitForSeconds(1f);
            ContinueGameLoop();
        }
        else
        {
            _gameplayUtility.EnableButtons(guessButtons);
            _gameplayUtility.Timer.StartGuessTimer();
        }
    }
    protected override void OnTimerExpired() => StartCoroutine(TimerExpiredRoutine());
    IEnumerator TimerExpiredRoutine()
    {
        _gameplayUtility.DisableButtons(guessButtons);

        AudioManager.PlaySound(AudioManager.timerExpired, SoundType.UI);

        yield return new WaitForSeconds(1f);
        _gameplayUtility.HideButtons(guessButtons);
        yield return new WaitForSeconds(1f);
        ContinueGameLoop();
    }
    #endregion

    #region HELPERS
    IEnumerator PlayIntro()
    {
        //StartCoroutine(_gameplayUtility.LoadButtonsRoutine(guessButtons, currentNotes.Count, .1f, false));
        yield return new WaitForSeconds(1f);
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
        UIAnimator.PulseTextSize(_gameUI.DroneText, 1.2f, 0.5f);
    }
    void StopDroneNoteEffect()
    {
        UIAnimator.StopTextPulse();
        //AudioManager.StopSound(droneNote, SoundType.DRONE);
    }
    IEnumerator PlayNotesRoutine()
    {
        currentNumGuessesGiven = 0;
        answers.Clear();

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
            _gameUI.DisplayDebugText(_gameplayUtility.GetIndianNotationFormatted(note)); // testing

            yield return new WaitForSeconds(2.5f);
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
    
    #endregion

    #region UTILITY

    public override void PauseGame()
    {
        AudioManager.PauseSound(droneNote, SoundType.DRONE);

        foreach (var sound in activeNoteSounds)
        {
            AudioManager.PauseSound(sound, SoundType.HARMONIUM);
        }
    }
    public override void ResumeGame()
    {
        AudioManager.UnPauseSound(droneNote, SoundType.DRONE);

        foreach (var sound in activeNoteSounds)
        {
            AudioManager.UnPauseSound(sound, SoundType.HARMONIUM);
        }
    }
    
    #endregion

}
