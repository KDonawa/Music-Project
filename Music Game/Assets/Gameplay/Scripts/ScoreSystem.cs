using System.Collections;
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
    int totalNumGuesses;
    public static ScoreSystem Instance { get; private set; }

    #region SETUP
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        if (scoreTextGUI) ShowTextGUI(scoreTextGUI, false);
        if (pointsGainedTextGUI) ShowTextGUI(pointsGainedTextGUI, false);
        if (scoreMultiTextGUI) ShowTextGUI(scoreMultiTextGUI, false);
    }

    private void Start()
    {
        GuessButton.GuessCorrectEvent += IncrementNumCorrectGuesses;
    }
    private void OnDestroy()
    {
        if(Instance == this) Instance = null;

        GuessButton.GuessCorrectEvent -= IncrementNumCorrectGuesses;
    }

    public void Initialize(int numGuesses)
    {
        //StopAllCoroutines();
        PlayerScore = 0f;
        numCorrectGuesses = 0;
        totalNumGuesses = numGuesses;

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
        //UpdateStreakAndMultiplier();
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

    #endregion

    #region UTILITY
    void IncrementNumCorrectGuesses() => numCorrectGuesses++;
    public int FinalScorePercentage()
    {
        float scorePercentage = numCorrectGuesses / (float)totalNumGuesses * 100f;
        return (int)scorePercentage;
    }
    //public void UpdatePlayerScore(int pointsGained) => StartCoroutine(UpdatePlayerScoreRoutine(pointsGained));  
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
