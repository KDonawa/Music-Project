using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using System;

public class GameplayType1 : LevelGameplay
{   
    int currentNoteIndex;    

    #region SETUP
    
    #endregion

    #region GAMEPLAY
    
    protected override void PlayGameLoop() => StartCoroutine(PlayGameLoopRountine());
    protected override void OnGuessButtonPressed(Button guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));
    void PlayCurrentNotes() => StartCoroutine(PlayCurrentNotesRoutine());
    #endregion

    #region HELPERS
    IEnumerator PlayGameLoopRountine()
    {   
        // play intro
        StartCoroutine(gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f, false));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < currentNotes.Count; i++)
        {
            Button b = guessButtons[i];
            gameplayUtility.ChangeButtonColor(b, Color.green);
            AudioManager.Instance.PlaySound(currentLevel.notes[i].name);
            yield return new WaitForSeconds(2.5f);
            gameplayUtility.ResetButtonColor(b);
            yield return new WaitForSeconds(0.2f);
        }
        gameplayUtility.HideButtons(guessButtons);

        //currentNoteIndex = 0;
        //gameplayUtility.RandomizeList(currentNotes);        
        //
        currentNoteIndex = UnityEngine.Random.Range(0, currentNotes.Count);
        PlayCurrentNotes();
    }
    IEnumerator PlayCurrentNotesRoutine()
    {
        droneText.text = "Playing " + droneNote;
        droneText.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound(droneNote);
        yield return new WaitForSeconds(4f);
        droneText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        droneText.text = "Playing Note";
        droneText.gameObject.SetActive(true);

        AudioManager.Instance.PlaySound(currentNotes[currentNoteIndex]);
        yield return new WaitForSeconds(2.2f);
        AudioManager.Instance.StopSound(droneNote);
        StartCoroutine(gameplayUtility.DisplayButtonsRoutine(guessButtons, currentNotes.Count, .1f));

        droneText.gameObject.SetActive(false);

        gameplayUtility.Timer.StartGuessTimer();
    }

    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        gameplayUtility.Timer.StopGuessTimer();
        gameplayUtility.EnableButtons(guessButtons, false);        

        bool isGuessCorrect = gameplayUtility.GetIndianNotation(currentNotes[currentNoteIndex], droneNote) == guessButton.GetComponentInChildren<TextMeshProUGUI>().text;
        if (isGuessCorrect)
        {
            gameplayUtility.ChangeButtonColor(guessButton, Color.green);
            AudioManager.Instance.PlaySound("correct guess");
            //GameplayUtility.ScoreSystem.UpdatePlayerScore(currentLevel.pointsPerCorrectGuess);            
        }
        else
        {
            gameplayUtility.ChangeButtonColor(guessButton, Color.red);
            AudioManager.Instance.PlaySound("wrong guess");
            //GameplayUtility.ScoreSystem.ResetStreakAndMultiplier();
        }
        gameplayUtility.ScoreSystem.UpdateGuessAccuracy(isGuessCorrect);
        gameplayUtility.TextSystem.DisplayGuessFeedback(isGuessCorrect);
        yield return new WaitForSeconds(1.5f);
        
        gameplayUtility.HideButtons(guessButtons);
        gameplayUtility.ResetButtonColor(guessButton);
        gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
        yield return new WaitForSeconds(1f);

        //StartNextGuessCycle();
        ContinueGameLoop();


    }
    void StartNextGuessCycle()
    {
        currentNoteIndex++;

        if (currentNoteIndex < currentNotes.Count)
        {
            PlayCurrentNotes();
        }
        else
        {
            ContinueGameLoop();
        }
    }

    #endregion

    #region UTILITY
    //public override bool IsLevelComplete() => currentNoteIndex == currentLevel.notes.Length;
    public override void PauseGame()
    {
        // if timer active return and do nothing
        gameplayUtility.Timer.StopGuessTimer();

        AudioManager.Instance.PauseSound(droneNote);
        AudioManager.Instance.PauseSound(currentNotes[currentNoteIndex]);
    }
    public override void ResumeGame()
    {
        AudioManager.Instance.UnPauseSound(droneNote);
        AudioManager.Instance.UnPauseSound(currentNotes[currentNoteIndex]);
    }
    #endregion


}
