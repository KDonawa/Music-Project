﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreTextGUI = null;
    [SerializeField] TextMeshProUGUI pointsGainedTextGUI = null;
    [SerializeField] TextMeshProUGUI scoreMultiTextGUI = null;
    
    public float PlayerScore { get; private set; }
    public float ScoreMultiplier { get; private set; }
    public int GuessStreak { get; private set; }

    int numCorrectGuesses;
    int numWrongGuesses;
    
    #region SETUP
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        StopAllCoroutines();
        PlayerScore = 0f;
        numCorrectGuesses = 0;
        numWrongGuesses = 0;
        ResetStreakAndMultiplier();
        UpdateScoreText();
        if (scoreTextGUI) ShowTextGUI(scoreTextGUI, false);
        if (pointsGainedTextGUI) ShowTextGUI(pointsGainedTextGUI, false);
        if (scoreMultiTextGUI) ShowTextGUI(scoreMultiTextGUI, false);
    }
    #endregion

    #region HELPER METHODS
    void UpdateScoreText()
    {
        scoreTextGUI.text = PlayerScore.ToString();
    }
    void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true)
    {
        textGUI.gameObject.SetActive(canShow);
    }
    IEnumerator UpdatePlayerScoreRoutine(int points)
    {
        UpdateStreakAndMultiplier();
        float pointsGained = points * ScoreMultiplier;
        PlayerScore += pointsGained;

        // display points gained text and updated score text
        pointsGainedTextGUI.text = "+" + pointsGained.ToString();
        ShowTextGUI(pointsGainedTextGUI);

        yield return new WaitForSeconds(1.5f);
        UpdateScoreText();

        
        yield return new WaitForSeconds(0.2f);
        ShowTextGUI(pointsGainedTextGUI, false);
    }
    void UpdateStreakAndMultiplier()
    {
        //GuessStreak++;

        //if (GuessStreak == 2) ScoreMultiplier = 2;
        //else if (GuessStreak == 3) DoubleMultiplier();
        //else if (GuessStreak == 4) DoubleMultiplier();
    }
    //void DoubleMultiplier() => ScoreMultiplier *= 2;
    #endregion

    #region UTILITY
    public void UpdateGuessAccuracy(bool isGuessCorrect)
    {
        if (isGuessCorrect) numCorrectGuesses++;
        else numWrongGuesses++;
    }
    public int GetPlayerScorePercentage()
    {
        float totalNumGuesses = numCorrectGuesses + numWrongGuesses;
        float scorePercentage = numCorrectGuesses / totalNumGuesses * 100f;
        return (int)scorePercentage;
    }
    public void UpdatePlayerScore(int pointsGained) => StartCoroutine(UpdatePlayerScoreRoutine(pointsGained));  
    public void DisplayScoreSystem(bool canDisplay = true)
    {
        if (scoreTextGUI) ShowTextGUI(scoreTextGUI, canDisplay);
    }
    public void ResetStreakAndMultiplier()
    {
        GuessStreak = 0;
        ScoreMultiplier = 1f;
    }
    #endregion
}
