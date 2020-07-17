using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayType2 : LevelGameplay
{
    
    int currentNumGuessesGiven;
    List<string> answers;
    //int currentNumAttemptsInGuessCycle;

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
        StartCoroutine(gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f, false));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < currentNotes.Count; i++)
        {
            Button b = guessButtons[i];
            gameplayUtility.ChangeButtonColor(b, Color.green);
            string noteToPlay = gameplayUtility.GetWesternNotation(currentLevel.subLevels[currentSubLevel].notes[i], droneNote);
            AudioManager.Instance.PlaySound(noteToPlay);
            yield return new WaitForSeconds(2.5f);
            gameplayUtility.ResetButtonColor(b);
            yield return new WaitForSeconds(0.2f);
        }
        gameplayUtility.HideButtons(guessButtons);
    }
    IEnumerator PlayGameLoopRoutine()
    {
        //yield return StartCoroutine(PlayIntro());
        yield return null;
        
        //currentNumAttemptsInGuessCycle = 0;
        PlayCurrentNotes();
    }
    IEnumerator PlayCurrentNotesRoutine()
    {
        currentNumGuessesGiven = 0;

        droneText.text = "Playing " + droneNote;
        droneText.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound(droneNote);

        yield return new WaitForSeconds(4f);
        droneText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        droneText.text = "Playing Notes";
        droneText.gameObject.SetActive(true);

        answers.Clear();
        for (int i = 0; i < numNotesPlayedPerGuess; i++)
        {
            string note = currentNotes[UnityEngine.Random.Range(0, currentNotes.Count)];
            answers.Add(note);
            AudioManager.Instance.PlaySound(gameplayUtility.GetWesternNotation(note, droneNote)); 
            yield return new WaitForSeconds(2.5f);
        }
        AudioManager.Instance.StopSound(droneNote);
        StartCoroutine(gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f));

        droneText.gameObject.SetActive(false);

        gameplayUtility.Timer.StartGuessTimer();
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        gameplayUtility.DisableButtons(guessButtons);
        currentNumGuessesGiven++;

        if(currentNumGuessesGiven == numNotesPlayedPerGuess)
        {
            gameplayUtility.Timer.StopGuessTimer();
            gameplayUtility.EnableButtons(guessButtons, false);
        }

        //bool isGuessCorrect = gameplayUtility.GetIndianNotation(answers[currentNumGuessesGiven-1], droneNote) == guessButton.GetComponentInChildren<TextMeshProUGUI>().text;
        bool isGuessCorrect = answers[currentNumGuessesGiven - 1] == guessButton.GetComponentInChildren<TextMeshProUGUI>().text;
        if (isGuessCorrect)
        {
            gameplayUtility.ChangeButtonColor(guessButton, Color.green);
            AudioManager.Instance.PlaySound("correct guess");            
        }
        else
        {
            gameplayUtility.ChangeButtonColor(guessButton, Color.red);
            AudioManager.Instance.PlaySound("wrong guess");            
        }
        yield return new WaitForSeconds(0.5f);
        gameplayUtility.ResetButtonColor(guessButton);

        gameplayUtility.ScoreSystem.UpdateGuessAccuracy(isGuessCorrect);
        gameplayUtility.TextSystem.DisplayGuessFeedback(isGuessCorrect);
        

        if (currentNumGuessesGiven == numNotesPlayedPerGuess)
        {
            yield return new WaitForSeconds(1.5f);
            gameplayUtility.ResetButtonColor(guessButton);
            gameplayUtility.HideButtons(guessButtons);
            gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
            yield return new WaitForSeconds(1f);

            ContinueGameLoop();
        }
        else
        {
            gameplayUtility.EnableButtons(guessButtons);
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
