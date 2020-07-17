using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// should be responsible for having all game effects and systems
// should have checks and balances for systems being used
public class LevelGameplayUtility : MonoBehaviour
{
    [SerializeField] Timer timerPrefab = null;
    [SerializeField] ScoreSystem scoreSystemPrefab = null;
    [SerializeField] TextSystem textSystemPrefab = null;

    public Timer Timer { get; private set; }
    public ScoreSystem ScoreSystem { get; private set; }
    public TextSystem TextSystem { get; private set; }

    string[] notesIN = { "Sa", "Re", "Ga", "Ma", "Pa", "Dha", "Ni", "SA" };
    string[] notesWN  = { "C3", "D", "E", "F", "G", "A", "B", "C4" };
    string[] droneNotes = { "Drone: C", "Drone: D", "Drone: E", "Drone: F", "Drone: G", "Drone: A", "Drone: B" };

    #region SETUP
    private void Awake()
    {
        Timer = Instantiate(timerPrefab);
        ScoreSystem = Instantiate(scoreSystemPrefab);
        TextSystem = Instantiate(textSystemPrefab);
    }
    #endregion

    #region UTILITY
    public string GetIndianNotation(string westernNotation, string droneNote)
    {
        int indexDrone = Array.FindIndex(droneNotes, x => x.Contains(droneNote)); // find the index of the drone note 
        int indexWN = Array.FindIndex(notesWN, x => x.Contains(westernNotation));
        int indexIN = (indexWN - indexDrone + notesIN.Length) % notesIN.Length;
        return notesIN[indexIN];
    }
    public string GetWesternNotation(string indianNotation, string droneNote)
    {
        int indexDrone = Array.FindIndex(droneNotes, x => x.Contains(droneNote)); // find the index of the drone note
        int indexIN = Array.FindIndex(notesIN, x => x.Contains(indianNotation));
        int indexWN = (indexIN - indexDrone + notesWN.Length) % notesWN.Length;
        return notesWN[indexWN];
    }
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
    public IEnumerator DisplayButtonsByNameRoutine(List<Button> buttonsList, int numberToDisplay, float timeBetweenButtons = 0f)
    {
        for (int i = 0; i < numberToDisplay; i++)
        {
            ResetButtonColor(buttonsList[i]);
            DisplayButton(buttonsList[i]);
            //if (buttonLoadSound) AudioManager.PlaySoundOneShot(buttonLoadSound.name);
            yield return new WaitForSeconds(timeBetweenButtons);
        }
    }
    public IEnumerator DisplayButtonsRoutine(List<Button> buttonsList, int numToDisplay, float timeBetweenButtons = 0f, bool canEnable = true)
    {
        for (int i = 0; i < numToDisplay; i++)
        {
            Button b = buttonsList[i];
            ResetButtonColor(b);
            DisplayButton(b, canEnable);
            //if (buttonLoadSound) AudioManager.PlaySoundOneShot(buttonLoadSound.name);
            yield return new WaitForSeconds(timeBetweenButtons);
        }
    }
    public void RandomizeList(List<string> currentNotes)
    {
        for (int index = currentNotes.Count - 1; index > 0; index--)
        {
            // Pick a random index from 0 to length of list
            int randInt = UnityEngine.Random.Range(0, currentNotes.Count);

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
    public void HideButtonsByName(List<Button> buttonsList, List<string> buttonNames)
    {
        foreach (var buttonName in buttonNames)
        {
            Button b = FindButtonByName(buttonsList, buttonName);
            HideButton(b);
        }
    }
    public void HideButtons(List<Button> buttonsList)
    {
        foreach (var b in buttonsList)
        {
            HideButton(b);
        }
    }
    public void EnableButtonsByName(List<Button> buttonsList, List<string> buttonNames, bool canEnable = true)
    {
        foreach (var buttonName in buttonNames)
        {
            EnableButton(FindButtonByName(buttonsList, buttonName), canEnable);
        }
    }
    public void EnableButtons(List<Button> buttonsList, bool canEnable = true)
    {
        foreach (var b in buttonsList)
        {
            EnableButton(b, canEnable);
        }
    }
    public void DisableButton(Button b)
    {
        EnableButton(b, false);
    }
    public void DisableButtons(List<Button> buttonsList)
    {
        EnableButtons(buttonsList, false);
    }
    #endregion

    #region HELPER METHODS
    void EnableButton(Button b, bool isInteractable = true)
    {
        if (b) b.interactable = isInteractable;
    }
    void DisplayButton(Button b, bool canEnable = true)
    {
        if (b)
        {
            EnableButton(b, canEnable);
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

}
