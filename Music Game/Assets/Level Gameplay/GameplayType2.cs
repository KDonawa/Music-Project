using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayType2 : LevelGameplay
{
    [SerializeField] string referenceSound = null;
    [SerializeField] string[] guessSounds = null;
    [SerializeField] GameObject guessButtonsHolder = null;
    [SerializeField] Button choiceButtonPrefab = null;
    [SerializeField] TextMeshProUGUI referenceTextGUI = null;

    [Range(1, 10)] [SerializeField] int numGuesses = 3;
    [SerializeField] int pointsPerCorrectGuess = 100;
    [SerializeField] int pointsLostPerIncorrectGuess = 30;
    [Range(50f, 100f)] [SerializeField] float levelPassPercentage = 75f;

    List<Button> guessButtonsList = new List<Button>();
    Coroutine playLoopSequenceRoutine = null;
    Coroutine guessButtonPressedRoutine = null;
    int currentGuessNoteIndex = 0;
    int guessesRemaining;
    List<bool> correctGuesses = new List<bool>();

    public string ReferenceSound { get { return referenceSound; } }
    public string CurrentGuessSound
    {
        get { return currentGuessNoteIndex < guessSounds.Length ? guessSounds[currentGuessNoteIndex] : string.Empty; }
    }

    protected override void Start()
    {
        base.Start();
        InitializeGuessButtons();
    }
    protected override void SetupLevel()
    {
        base.SetupLevel();
        GameplayUtility.Timer.EnableTimer();
        GameplayUtility.ScoreSystem.EnableScoreSystem();

        referenceTextGUI.gameObject.SetActive(false);
        guessButtonsHolder.SetActive(true);
        EnableGuessButtons(false);
        currentGuessNoteIndex = 0;
        ResetLives();
    }
    protected override void StartGameLoop()
    {
        GameplayUtility.Timer.StartCountdown(3);
    }
    protected override void OnTimerExpired() { PlayNextNoteSequence(); }
    protected override void OnCountdownCompleted() { PlayNoteSequence(); }
    void PlayNoteSequence()
    {
        if (playLoopSequenceRoutine != null) { StopCoroutine(playLoopSequenceRoutine); }
        StartCoroutine(StartLoopSequenceRoutine());
    }
    void PlayNextNoteSequence()
    {
        currentGuessNoteIndex++;
        if (IsLevelComplete())
        {
            StartCoroutine(EndLevel());
        }
        else
        {
            GameplayUtility.Timer.ResetGuessTimer(timePerGuess);    // make an event OnPlayNextNoteEvent
            ResetLives();
            HideGuessButtons();
            PlayNoteSequence();
        }
    }
    IEnumerator StartLoopSequenceRoutine()
    {
        if (IsFirstGuess())
        {
            DisplayReferenceText();

            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(DisplayGuessButtons(0.5f));
        }

        playLoopSequenceRoutine = StartCoroutine(PlayLoopSequenceRoutine(currentGuessNoteIndex, false));
    }
    IEnumerator PlayLoopSequenceRoutine(int guessIndex, bool isReplay)
    {
        AudioManager.PlaySound(referenceSound);
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameplayUtility.GrowAndShrinkTextRoutine(referenceTextGUI, 130f, 1f));
        yield return new WaitForSeconds(1f);

        AudioManager.PlaySound(guessSounds[guessIndex]);
        yield return new WaitForSeconds(1f);
        foreach (var button in guessButtonsList)
        {
            if (button.IsActive())
                StartCoroutine(GameplayUtility.GrowAndShrinkTextRoutine(button.GetComponentInChildren<TextMeshProUGUI>(), 90f, 1f));
        }
        if (!isReplay)
        {
            EnableGuessButtons(true);
            GameplayUtility.Timer.StartGuessTimer(); // OnStartTimer Event
        }

        yield return new WaitForSeconds(1.5f);

        if (GameplayUtility.Timer.RemainingTime >= 4.5f) // remove timer check and have the manager stop it
        {
            playLoopSequenceRoutine = StartCoroutine(PlayLoopSequenceRoutine(guessIndex, true));
        }
    }
    IEnumerator GuessButtonPressedRoutine(Button guessButton)
    {
        EnableGuessButtons(false);
        GameplayUtility.Timer.StopGuessTimer();

        if (playLoopSequenceRoutine != null) StopCoroutine(playLoopSequenceRoutine);
        AudioManager.StopSound(referenceSound);

        bool guessCorrect = guessButton.GetComponentInChildren<TextMeshProUGUI>().text == guessSounds[currentGuessNoteIndex];
        if (guessCorrect)
        {
            correctGuesses.Add(true);
            DisplayReferenceText(false);

            GameplayUtility.ChangeButtonColor(guessButton, Color.green);
            yield return new WaitForSeconds(1f);

            // calculate Player Score
            int livesLost = numGuesses - guessesRemaining;
            int pointsGained = pointsPerCorrectGuess - (livesLost * pointsLostPerIncorrectGuess);
            GameplayUtility.ScoreSystem.UpdatePlayerScore(pointsGained);
            yield return new WaitForSeconds(1.5f);

            PlayNextNoteSequence();
        }
        else
        {
            GameplayUtility.ChangeButtonColor(guessButton, Color.red);
            yield return new WaitForSeconds(1f);
            guessButton.gameObject.SetActive(false);

            --guessesRemaining;
            if (guessesRemaining == 0)
            {
                correctGuesses.Add(false);
                yield return StartCoroutine(ShowCorrectNoteRoutine());
                PlayNextNoteSequence();
            }
            else
            {
                PlayNoteSequence();
            }
        }
    }
    IEnumerator ShowCorrectNoteRoutine()
    {
        //yield return StartCoroutine(DisplayGuessButtons());

        //AudioManager.PlaySound(guessSounds[currentGuessNoteIndex], true);
        //yield return new WaitForSeconds(1f);

        Button correctButton = GetCorrectButton();
        if (correctButton != null)
        {
            GameplayUtility.ChangeButtonColor(correctButton, Color.green);
        }
        yield return new WaitForSeconds(1f);
    }
    IEnumerator PlayEndSequenceRoutine()
    {
        DisplayReferenceText(false);
        yield return StartCoroutine(DisplayGuessButtons(0.3f));

        for (int i = 0; i < guessSounds.Length; i++)
        {
            AudioManager.PlaySound(referenceSound);
            yield return new WaitForSeconds(0.8f);

            AudioManager.PlaySound(guessSounds[i]);
            yield return new WaitForSeconds(0.8f);

            if (correctGuesses[i]) { GameplayUtility.ChangeButtonColor(guessButtonsList[i], Color.green); }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnGuessButtonPressed(Button guessButton)
    {
        if (guessButtonPressedRoutine != null) StopCoroutine(guessButtonPressedRoutine);
        guessButtonPressedRoutine = StartCoroutine(GuessButtonPressedRoutine(guessButton));
    }
    void HideGuessButtons()
    {
        foreach (Button button in guessButtonsList)
        {
            button.gameObject.SetActive(false);
        }
    }
    void EnableGuessButtons(bool value)
    {
        foreach (Button button in guessButtonsList)
        {
            button.interactable = value;
        }
    }
    void DisplayReferenceText(bool canDisplay = true)
    {
        referenceTextGUI.gameObject.SetActive(canDisplay);
        if (canDisplay) { referenceTextGUI.text = referenceSound; }
    }
    IEnumerator DisplayGuessButtons(float timeBetweenButtons = 0f, bool enableButtons = false)
    {
        foreach (Button button in guessButtonsList)
        {
            button.interactable = enableButtons;
            button.GetComponent<Image>().color = Color.white;
            button.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeBetweenButtons);
        }
    }


    #region HELPER METHODS
    void InitializeGuessButtons()
    {
        guessButtonsList.Clear();
        foreach (var sound in guessSounds)
        {
            Button b = Instantiate(choiceButtonPrefab, guessButtonsHolder.transform);
            b.gameObject.SetActive(false);

            TextMeshProUGUI textGUI = b.GetComponentInChildren<TextMeshProUGUI>();
            textGUI.text = sound;

            b.onClick.AddListener(delegate { OnGuessButtonPressed(b); });

            guessButtonsList.Add(b);
        }
    }
    Button GetCorrectButton()
    {
        foreach (var button in guessButtonsList)
        {
            if (button.GetComponentInChildren<TextMeshProUGUI>().text == guessSounds[currentGuessNoteIndex])
                return button;
        }
        return null;
    }
    public override void PauseGame()
    {
        if (!IsLevelComplete())
        {
            AudioManager.PauseSound(ReferenceSound);
            AudioManager.PauseSound(CurrentGuessSound);
        }
    }
    public override void ResumeGame()
    {
        if (!IsLevelComplete())
        {
            AudioManager.ResumeSound(ReferenceSound);
            AudioManager.ResumeSound(CurrentGuessSound);
        }
    }
    void ResetLives()
    {
        guessesRemaining = numGuesses;
    }
    IEnumerator EndLevel()
    {
        // maybe PlayEndSequenceRoutine when game is over instead of calling end level

        //yield return StartCoroutine(PlayEndSequenceRoutine());
        yield return new WaitForSeconds(2f);

        HideGuessButtons();
        DisplayReferenceText(false);

        //GameplayUtility.restartButton.gameObject.SetActive(true);

        if (IsLevelPassed())
        {
            print("Level passed!");
            // save level complete data
        }
        else
        {
            print("Try again");
        }

        print("Player score: " + GameplayUtility.ScoreSystem.PlayerScore);
    }
    #endregion

    #region BOOLEAN METHODS
    public override bool IsLevelComplete()
    {
        return currentGuessNoteIndex >= guessSounds.Length;
    }
    public override bool IsLevelPassed()
    {
        float maxScore = currentGuessNoteIndex * pointsPerCorrectGuess;
        float scorePercentage = GameplayUtility.ScoreSystem.PlayerScore / maxScore * 100f;
        return scorePercentage >= levelPassPercentage;
    }
    bool IsFirstGuess() { return guessesRemaining == numGuesses; }
    #endregion
}
