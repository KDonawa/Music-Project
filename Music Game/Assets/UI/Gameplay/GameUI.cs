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

    public TextMeshProUGUI LevelText => levelText;
    public TextMeshProUGUI DroneText => droneText;
    public TextMeshProUGUI GameText => gameText;
    public TextMeshProUGUI DebugText => debugText;
    public GameObject GuessButtonsContainer => guessButtonsContainer;
    public Button GuessButton => guessButton;

    private void Awake()
    {
        Inititialize();
    }

    public void Inititialize()
    {
        if (levelText != null) levelText.gameObject.SetActive(false);
        if (droneText != null) droneText.gameObject.SetActive(false);
        if (gameText != null) gameText.gameObject.SetActive(false);
        if (debugText != null) debugText.gameObject.SetActive(false);
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
