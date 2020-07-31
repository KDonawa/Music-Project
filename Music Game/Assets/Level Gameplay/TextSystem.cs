using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textGUI = null;
    [SerializeField] string[] corerctGuessMessages = null;
    [SerializeField] string[] wrongGuessMessages = null;

    #region SETUP
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        StopAllCoroutines();
        if (textGUI) ShowTextGUI(textGUI, false);
    }
    #endregion

    #region UTILITY
    public void DisplayGuessFeedback(bool isGuessCorrect) => StartCoroutine(DisplayGuessFeedbackRoutine(isGuessCorrect));

    #endregion

    #region HELPER METHODS

    void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true)
    {
        textGUI.gameObject.SetActive(canShow);
    }
    IEnumerator DisplayGuessFeedbackRoutine(bool isGuessCorrect)
    {
        if (isGuessCorrect)
        {
            if(corerctGuessMessages.Length > 0)
            {
                int randInt = UnityEngine.Random.Range(0, corerctGuessMessages.Length);
                textGUI.text = corerctGuessMessages[randInt];
            }            
        }
        else
        {
            if (wrongGuessMessages.Length > 0)
            {
                int randInt = UnityEngine.Random.Range(0, wrongGuessMessages.Length);
                textGUI.text = wrongGuessMessages[randInt];
            }
        }

        ShowTextGUI(textGUI);
        yield return new WaitForSeconds(1f);
        ShowTextGUI(textGUI, false);
    }
    #endregion
}
