using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

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

    // lower octaves: .sa .re .ga (must be bold and italicized)
    // higher octaves: 'sa 're 'ga (end in apostrophe and Capitalized)
    // drone: d:C3 -> 3 will tell us the octave
    readonly string[] indianNotes = { "sa", "_re", "re", "_ga", "ga", "ma", "Ma", "pa", "_dha", "dha", "_ni", "ni" };
    readonly string[] westernNotes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    readonly string[] droneNotes = { "d:C", "d:C#", "d:D", "d:D#", "d:E", "d:F", "d:F#", "d:G", "d:G#", "d:A", "d:A#", "d:B", };

    StringBuilder sb;
    
    #region SETUP
    private void Awake()
    {
        sb = new StringBuilder(30);
        Timer = Instantiate(timerPrefab);
        ScoreSystem = Instantiate(scoreSystemPrefab);
        TextSystem = Instantiate(textSystemPrefab);
    }
    #endregion

    #region UTILITY
    public string GetIndianNotation(string westernNotation, string droneNote)
    {
        //int indexDrone = Array.FindIndex(droneNotes, x => x.Contains(droneNote)); // find the index of the drone note 
        //int indexWN = Array.FindIndex(notesWN, x => x.Contains(westernNotation));
        //int indexIN = (indexWN - indexDrone + notesIN.Length) % notesIN.Length;
        //return notesIN[indexIN];
        return string.Empty;
    }
    public string GetDroneNoteFormatted(string droneNote)
    {
        int octave = droneNote[droneNote.Length - 1] - '0';
        droneNote = droneNote.Substring(0, droneNote.Length - 1);
        return westernNotes[Array.FindIndex(droneNotes, x => x == droneNote)] + octave;
    }
    public string GetIndianNotationFormatted(string indianNotation)
    {
        bool isBoldAndItalicized = false;
        bool isUnderlined = false;
        bool isCapitalized = false;

        if (indianNotation[0] == '.')
        {
            indianNotation = indianNotation.Substring(1);
            isBoldAndItalicized = true;
        }
        else if (indianNotation[0] == '\'')
        {
            isCapitalized = true;
            indianNotation = indianNotation.Substring(1) + '\'';
        }
        if(indianNotation[0] == '_')
        {
            isUnderlined = true;
            indianNotation = indianNotation.Substring(1);
        }
        if (isCapitalized)
        {
            indianNotation = char.ToUpper(indianNotation[0]) + indianNotation.Substring(1);
        }

        sb.Clear();
        if (isBoldAndItalicized) sb.Append("<i><b>");
        if (isUnderlined) sb.Append("<u>");
        sb.Append(indianNotation);
        if (isUnderlined) sb.Append("</u>");
        if (isBoldAndItalicized) sb.Append("</b></i>");

        return sb.ToString();
    }
    public string GetWesternNotation(string indianNotation, string droneNote)
    {
        int octave = droneNote[droneNote.Length - 1] - '0';        
        droneNote = droneNote.Substring(0, droneNote.Length - 1);
        if (indianNotation[0] == '.')
        {
            octave--;
            indianNotation = indianNotation.Substring(1);
        }
        else if (indianNotation[0] == '\'')
        {
            octave++;
            indianNotation = indianNotation.Substring(1);
        }
        //Debug.Log("octave: " + octave);

        int indexDrone = Array.FindIndex(droneNotes, x => x == droneNote); // find the index of the drone note
        //Debug.Log("drone note: " + droneNote);
        //Debug.Log("drone index: " + indexDrone);

        int indexIN = Array.FindIndex(indianNotes, x => x == indianNotation);
        //Debug.Log("indian note: " + indianNotation);
        //Debug.Log("IN index: " + indexIN);

        int indexWN = (indexIN - indexDrone + westernNotes.Length) % westernNotes.Length;
        //Debug.Log("WN index: " + indexWN);
        return westernNotes[indexWN] + octave;
    }
    public IEnumerator GrowAndShrinkTextRoutine(TextMeshProUGUI textGUI, float growthAmount, float period)
    {
        float originalTextSize = textGUI.fontSize;
        float growValue = originalTextSize * growthAmount;

        while (true)
        {
            float elapsedTime = 0f;
            while (elapsedTime < period / 2)
            {
                textGUI.fontSize = Mathf.Lerp(textGUI.fontSize, growValue, Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            elapsedTime = 0f;
            while (elapsedTime < period / 2)
            {
                textGUI.fontSize = Mathf.Lerp(textGUI.fontSize, originalTextSize, Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            //textGUI.fontSize = originalTextSize;
        }        
        
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
        if (b) b.GetComponent<Image>().color = Color.black;
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
