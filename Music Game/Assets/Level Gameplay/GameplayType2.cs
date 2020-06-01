using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayType2 : LevelGameplay
{


    #region SETUP
    protected override void SetupLevel()
    {
        base.SetupLevel();

    }
    #endregion

    #region GAMEPLAY

    protected override void PlayGameLoop() => StartCoroutine(PlayGameLoopRoutine());
    protected override void OnGuessButtonPressed(Button guessButton) => StartCoroutine(GuessButtonPressedRoutine(guessButton));

    #endregion

    #region HELPERS

    IEnumerator PlayGameLoopRoutine()
    {
        yield return null;
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        yield return null;
    }
    #endregion

    #region UTILITY
    public override bool IsLevelComplete()
    {
        //TODO
        return false;
    }
    public override void PauseGame()
    {

    }
    public override void ResumeGame()
    {

    }
    #endregion

}
