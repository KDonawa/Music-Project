using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText = null;
    [SerializeField] TextMeshProUGUI droneText = null;
    [SerializeField] TextMeshProUGUI gameText = null;
    [SerializeField] TextMeshProUGUI debugText = null;
    [SerializeField] GameObject guessButtonsContainer = null;
    [SerializeField] Button guessButton = null;

    float originalDroneTextSize;

    IEnumerator dronePulseRoutine;

    private void Awake()
    {        
        Inititialize();        
    }
    public void Inititialize()
    {        
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

    public IEnumerator DisplayCurrentLevelRoutine()    
    {       
        levelText.text = "Level " + GameManager.Instance.currentLevel;
        levelText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);

        UIAnimator.ShrinkToNothing(levelText.rectTransform, 0.5f);
        yield return new WaitForSeconds(0.5f);    
        
    }
}
