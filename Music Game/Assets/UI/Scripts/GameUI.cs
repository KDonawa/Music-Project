using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button pauseButton = null;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI countdownTextGUI = null;
    [SerializeField] TextMeshProUGUI levelText = null;
    [SerializeField] TextMeshProUGUI droneText = null;
    [SerializeField] TextMeshProUGUI gameText = null;
    [SerializeField] TextMeshProUGUI debugText = null;

    [Header("Prefabs")]
    [SerializeField] Button guessButton = null;

    [Header("Containers")]    
    [SerializeField] GameObject guessButtonsContainer = null;
    

    float originalDroneTextSize;

    IEnumerator dronePulseRoutine;

    #region SETUP
    private void Awake()
    {        
        Inititialize();        
    }
    private void Start()
    {
        if (pauseButton == null) Debug.LogError("No reference to Pause button");
        else pauseButton.onClick.AddListener(OnPausePressed);
    }
    private void OnDestroy()
    {
        if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPausePressed);
    }
    
    public void Inititialize()
    {
        //StopAllCoroutines();
        originalDroneTextSize = droneText.fontSize;
        HideTextGUI(levelText);
        HideTextGUI(countdownTextGUI);
        HideDroneText();
        HideGameText();
        HideDebugText();
    }
    public Button InitGuessButton(string name, string textToDisplay)
    {
        if (guessButtonsContainer != null && guessButton != null)
        {
            Button b = Instantiate(guessButton, guessButtonsContainer.transform);
            b.gameObject.SetActive(false);
            b.GetComponent<GuessButton>().Initialize(name);
            b.GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay;
            b.onClick.AddListener(b.GetComponent<GuessButton>().ButtonPressed);
            return b;
        }
        return null;
    }
    #endregion

    #region COUNTDOWN
    public void StartCountdown(int startTime, float delay, System.Action endOfCountdownDelegate)
    {
        StartCoroutine(CountdownRoutine(startTime, delay, endOfCountdownDelegate));
    }
    IEnumerator CountdownRoutine(int startTime, float delay, System.Action endOfCountdownDelegate)
    {
        yield return new WaitForSeconds(1f); // this delay is needed for smoother start
        ShowTextGUI(countdownTextGUI);
        while (startTime > 0)
        {
            countdownTextGUI.text = startTime.ToString();
            AudioManager.PlaySound(AudioManager.countdown, SoundType.UI);
            yield return new WaitForSeconds(delay);
            startTime--;
        }
        HideTextGUI(countdownTextGUI);
        endOfCountdownDelegate?.Invoke();
    }

    #endregion
    

    #region EFFECTS
    public void PulseDroneText()
    {
        StopDronePulse();
        dronePulseRoutine = UIAnimator.PulseTextSizeRoutine(droneText, 1.2f, 0.5f);
        StartCoroutine(dronePulseRoutine);
    }
    public void StopDronePulse()
    {
        if (dronePulseRoutine != null) StopCoroutine(dronePulseRoutine);
    }
    public IEnumerator DisplayCurrentLevelRoutine()
    {
        levelText.text = string.Concat("Level ", GameManager.Instance.CurrentLevelIndex.ToString());
        ShowTextGUI(levelText);

        yield return new WaitForSeconds(2f);

        AudioManager.PlaySound(AudioManager.swoosh1, SoundType.UI);
        UIAnimator.ShrinkToNothing(levelText.rectTransform, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
    #endregion

    #region TEXT DISPLAY
    public void DisplayGameText(string textToDisplay)
    {       
        gameText.text = textToDisplay;
        ShowTextGUI(gameText);
    }
    public void DisplayDroneText(string textToDisplay)
    {
        droneText.fontSize = originalDroneTextSize;
        droneText.text = textToDisplay;
        ShowTextGUI(droneText);
    }
    public void DisplayDebugText(string textToDisplay)
    {
        //debugText.text += textToDisplay + ' ';
        debugText.text += string.Concat(textToDisplay, ' ');
        ShowTextGUI(debugText);
    }
    public void HideGameText() => HideTextGUI(gameText);

    public void HideDroneText()=> HideTextGUI(droneText);
    public void HideDebugText()
    {
        HideTextGUI(debugText);
        debugText.text = string.Empty;
    }

    #endregion

    void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true) => textGUI.gameObject.SetActive(canShow);
    void HideTextGUI(TextMeshProUGUI textGUI) => ShowTextGUI(textGUI, false);
    void OnPausePressed()
    {
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        //Time.timeScale = 0f;
        GameManager.ChangeGameState(GameState.Paused);
        //Game.PauseGame();
    }
}
