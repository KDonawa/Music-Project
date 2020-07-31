using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayType2 : LevelGameplay
{ 
    int currentNumGuessesGiven;
    List<string> answers;

    #region SETUP
    protected override void SetupLevel()
    {
        base.SetupLevel();
        answers = new List<string>();        
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
    IEnumerator PlayCurrentNotesRoutine()
    {
        currentNumGuessesGiven = 0;

        _gameUI.DroneText.text = "Drone: " + _gameplayUtility.GetDroneNoteFormatted(droneNote);
        _gameUI.DroneText.gameObject.SetActive(true);

        //_gameUI.GameText.text = "Playing Drone";
        //_gameUI.GameText.gameObject.SetActive(true);
        ////AudioManager.Instance.PlaySound(droneNote);
        ////yield return new WaitForSeconds(4f);
        //_gameUI.GameText.gameObject.SetActive(false);
        ////yield return new WaitForSeconds(1f);

        _gameUI.GameText.text = "Playing Notes";
        _gameUI.GameText.gameObject.SetActive(true);

        answers.Clear();

        for (int i = 0; i < numNotesPlayedPerGuess; i++)
        {
            string note = currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)];
            answers.Add(note);
            //AudioManager.Instance.PlaySound(gameplayUtility.GetWesternNotation(note, droneNote)); 
            _gameUI.DebugText.text = "Answer: " + _gameplayUtility.GetIndianNotationFormatted(note); // testing
            _gameUI.DebugText.gameObject.SetActive(true);
           yield return new WaitForSeconds(2.5f);
            _gameUI.DebugText.gameObject.SetActive(false);

        }
        AudioManager.Instance.StopSound(droneNote);
        StartCoroutine(_gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f));

        _gameUI.DroneText.gameObject.SetActive(false);
        _gameUI.GameText.gameObject.SetActive(false);
        //_gameUI.DebugText.gameObject.SetActive(false);

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

    }
    public override void ResumeGame()
    {

    }
    #endregion

}
