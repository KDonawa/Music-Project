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

    #region FORMAT NOTES
    public string GetIndianNotation(string westernNotation, string droneNote)
    {
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
    #endregion

    #region UTILITY

    public IEnumerator LoadButtonsRoutine(List<Button> buttonsList, float timeBetweenButtons, bool canEnable = true)
    {
        for (int i = 0; i < buttonsList.Count; i++)
        {
            LoadButton(buttonsList[i], canEnable);
            yield return new WaitForSeconds(timeBetweenButtons);
        }
    }
    public void LoadButtons(List<Button> buttonsList, float timeBetweenButtons = 0f, bool canEnable = true)
    {
        StartCoroutine(LoadButtonsRoutine(buttonsList, timeBetweenButtons, canEnable));
    }
    public void LoadButton(Button b, bool canEnable = true)
    {
        UIAnimator.SetColor(b.GetComponent<Image>(), Color.black);
        DisplayButton(b, canEnable);
        AudioManager.PlaySound(AudioManager.buttonLoad, SoundType.UI);
    }  
    public void HideButtons(List<Button> buttonsList)
    {
        foreach (var b in buttonsList)
        {
            HideButton(b);
        }
    }
    public void EnableButtons(List<Button> buttonsList, bool canEnable = true)
    {
        foreach (var b in buttonsList)
        {
            EnableButton(b, canEnable);
        }
    }
    public void DisableButtons(List<Button> buttonsList)
    {
        EnableButtons(buttonsList, false);
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
