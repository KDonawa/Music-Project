using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button pauseButton = null;

    [Header("Text")]
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
        if (levelText != null) levelText.gameObject.SetActive(false);
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
        levelText.text = "Level " + GameManager.Instance.currentLevel;
        levelText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        AudioManager.PlaySound(AudioManager.swoosh1, SoundType.UI);
        UIAnimator.ShrinkToNothing(levelText.rectTransform, 0.5f);
        yield return new WaitForSeconds(0.5f);

    }
    #endregion

    #region TEXT DISPLAY
    public void DisplayGameText(string textToDisplay)
    {       
        if (gameText == null) return;
        gameText.text = textToDisplay;
        gameText.gameObject.SetActive(true);
    }
    public void DisplayDroneText(string textToDisplay)
    {
        if (droneText == null) return;
        droneText.text = textToDisplay;
        droneText.gameObject.SetActive(true);

    }
    public void DisplayDebugText(string textToDisplay)
    {
        if (debugText == null) return;
        debugText.text += textToDisplay + ' ';
        debugText.gameObject.SetActive(true);

    }
    public void HideGameText()
    {
        if (gameText == null) return;
        gameText.gameObject.SetActive(false);
    }
    public void HideDroneText()
    {
        if (droneText == null) return;
        droneText.gameObject.SetActive(false);
        ResetDroneText();
    }
    public void HideDebugText()
    {
        if (debugText == null) return;
        debugText.gameObject.SetActive(false);
        debugText.text = string.Empty;
    }

    public void ResetDroneText()
    {
        droneText.fontSize = originalDroneTextSize;
    }
    #endregion

    void OnPausePressed()
    {
        //UIAnimator.ButtonPressEffect(pauseButton, AudioManager.click1);
        AudioManager.PlaySound(AudioManager.click1, SoundType.UI);
        Time.timeScale = 0f;

        Game.PauseGame();
    }
}
