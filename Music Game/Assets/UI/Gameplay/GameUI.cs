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

    //public TextMeshProUGUI LevelText => levelText;
    public TextMeshProUGUI DroneText => droneText;
    //public TextMeshProUGUI GameText => gameText;
    //public TextMeshProUGUI DebugText => debugText;
    //public GameObject GuessButtonsContainer => guessButtonsContainer;
    //public Button GuessButton => guessButton;

    private void Awake()
    {
        Inititialize();
        originalDroneTextSize = droneText.fontSize;
    }
    public void Inititialize()
    {
        if (levelText != null) levelText.gameObject.SetActive(false);
        HideDroneText();
        HideGameText();
        HideDebugText();
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

    public Button InstantiateGuessButton()
    {
        if(guessButtonsContainer != null && guessButton != null)
        {
            return Instantiate(guessButton, guessButtonsContainer.transform);
        }
        return null;
    }
    public IEnumerator DisplayCurrentLevelRoutine()
    {
        levelText.text = "Level " + GameManager.Instance.currentLevel;
        levelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        levelText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
    }
}
