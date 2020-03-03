using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// should be responsible for having all game effects and systems
// should have checks and balances for systems being used
public class LevelGameplayUtility : MonoBehaviour
{
    [SerializeField] Timer timerPrefab = null;
    [SerializeField] ScoreSystem scoreSystemPrefab = null;
    [SerializeField] Sound buttonLoadSound = null;

    public Timer Timer { get; private set; }
    public ScoreSystem ScoreSystem { get; private set; }

    // events
    public event System.Action StartGameEvent;

    private void Awake()
    {
        Timer = Instantiate(timerPrefab).GetComponent<Timer>();
        ScoreSystem = Instantiate(scoreSystemPrefab).GetComponent<ScoreSystem>();
        AudioManager.AddSound(buttonLoadSound);
    }


    #region UTILITY
    public IEnumerator GrowAndShrinkTextRoutine(TextMeshProUGUI textGUI, float growValue, float duration)
    {
        float originalTextSize = textGUI.fontSize;

        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            textGUI.fontSize = Mathf.Lerp(textGUI.fontSize, growValue, Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            textGUI.fontSize = Mathf.Lerp(textGUI.fontSize, originalTextSize, Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        textGUI.fontSize = originalTextSize;
    }
    public IEnumerator DisplayButtonsRoutine(List<Button> buttonsList, int numberToDisplay, float timeBetweenButtons = 0f)
    {
        for (int i = 0; i < numberToDisplay; i++)
        {
            ResetButtonColor(buttonsList[i]);
            DisplayButton(buttonsList[i]);
            //if (buttonLoadSound) AudioManager.PlaySoundOneShot(buttonLoadSound.name);
            yield return new WaitForSeconds(timeBetweenButtons);
        }
    }
    public void RandomizeList(List<string> currentNotes)
    {
        for (int index = currentNotes.Count - 1; index > 0; index--)
        {
            // Pick a random index from 0 to length of list
            int randInt = Random.Range(0, currentNotes.Count);

            // Swap ith element with the element at random index 
            string temp = currentNotes[index];
            currentNotes[index] = currentNotes[randInt];
            currentNotes[randInt] = temp;
        }
        //return currentNotes;
    }
    public void ResetButtonColor(Button b)
    {
        if (b) b.GetComponent<Image>().color = Color.white;
    }
    public void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }
    public void HideButtons(List<Button> buttonsList, List<string> buttonNames)
    {
        foreach (var buttonName in buttonNames)
        {
            Button b = FindButtonByName(buttonsList, buttonName);
            HideButton(b);
        }
    }
    public void EnableButtons(List<Button> buttonsList, List<string> buttonNames, bool canEnable = true)
    {
        foreach (var buttonName in buttonNames)
        {
            EnableButton(FindButtonByName(buttonsList, buttonName), canEnable);
        }
    }
    public void RestartGame()
    {
        OnStartGamePressed();
    }
    #endregion

    #region HELPER METHODS
    void EnableButton(Button b, bool isInteractable = true)
    {
        if (b) b.interactable = isInteractable;
    }
    void DisplayButton(Button b)
    {
        if (b)
        {
            EnableButton(b);
            //TextMeshProUGUI textGUI = b.GetComponentInChildren<TextMeshProUGUI>();
            //textGUI.text = name;
            b.gameObject.SetActive(true);
        }
    }
    void HideButton(Button b)
    {
        if (b)
        {
            b.gameObject.SetActive(false);
        }
    }

    Button FindButtonByName(List<Button> buttonsList, string name) =>
        buttonsList.Find(button => button.GetComponentInChildren<TextMeshProUGUI>().text == name);
    #endregion

    #region BUTTON EVENTS 
    public void OnStartGamePressed()
    {
        StartGameEvent?.Invoke();
    }
    
    #endregion
}
