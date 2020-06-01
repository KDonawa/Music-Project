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
    protected override void SetupLevel()
    {
        base.SetupLevel();        
        currentNoteIndex = 0;
    }    
    
    #endregion

    #region GAMEPLAY
    
    protected override void PlayGameLoop() => StartCoroutine(PlayGameLoopRoutine());    
    protected override void OnGuessButtonPressed(Button guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));
    
    #endregion

    #region HELPERS
    IEnumerator PlayGameLoopRoutine()
    {
        droneText.text = "Playing " + droneNote;
        droneText.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound(droneNote);
        yield return new WaitForSeconds(4f);
        droneText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        droneText.text = "Playing Note";
        droneText.gameObject.SetActive(true);

        AudioManager.Instance.PlaySound(notesInLevel[currentNoteIndex]);
        yield return new WaitForSeconds(2.2f);
        AudioManager.Instance.StopSound(droneNote);
        StartCoroutine(gameplayUtility.DisplayButtonsRoutine(guessButtons, .1f));

        droneText.gameObject.SetActive(false);

        gameplayUtility.Timer.StartGuessTimer();
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        //AudioManager.Instance.StopSound(droneNote);
        //AudioManager.Instance.StopSound(currentNotes[currentNoteIndex]);
        gameplayUtility.Timer.StopGuessTimer();
        gameplayUtility.EnableButtons(guessButtons, false);        

        bool isGuessCorrect = gameplayUtility.GetIndianNotation(notesInLevel[currentNoteIndex], droneNote) == guessButton.GetComponentInChildren<TextMeshProUGUI>().text;
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
        //yield return new WaitForSeconds(1f);
        gameplayUtility.HideButtons(guessButtons);
        gameplayUtility.ResetButtonColor(guessButton);
        gameplayUtility.Timer.ResetGuessTimer(timePerGuess);
        yield return new WaitForSeconds(1f);

        currentNoteIndex++;
        ContinueGameLoop();
    }
    

    #endregion

    #region UTILITY
    public override bool IsLevelComplete() => currentNoteIndex == currentLevel.notes.Length;
    public override void PauseGame()
    {
        // if timer active return and do nothing
        gameplayUtility.Timer.StopGuessTimer();

        AudioManager.Instance.PauseSound(droneNote);
        AudioManager.Instance.PauseSound(notesInLevel[currentNoteIndex]);
    }
    public override void ResumeGame()
    {
        AudioManager.Instance.UnPauseSound(droneNote);
        AudioManager.Instance.UnPauseSound(notesInLevel[currentNoteIndex]);
    }
    #endregion


}
