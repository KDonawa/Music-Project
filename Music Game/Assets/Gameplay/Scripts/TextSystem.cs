using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KD.MusicGame.Gameplay
{
    public class TextSystem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textGUI = null;
        [SerializeField] string[] corerctGuessMessages = null;
        [SerializeField] string[] wrongGuessMessages = null;
        [SerializeField] Color correctGuessColor = new Color();
        [SerializeField] Color wrongGuessColor = new Color();

        Coroutine _feedbackRoutine;

        #region SETUP
        private void Awake()
        {
            GuessButton.GuessCorrectEvent += DisplayCorrectGuessFeedback;
            GuessButton.GuessIncorrectEvent += DisplayIncorrectGuessFeedback;
            Initialize();
        }
        private void OnDestroy()
        {
            GuessButton.GuessCorrectEvent -= DisplayCorrectGuessFeedback;
            GuessButton.GuessIncorrectEvent -= DisplayIncorrectGuessFeedback;
        }
        public void Initialize()
        {
            StopAllCoroutines();
            if (textGUI) ShowTextGUI(textGUI, false);

        }
        #endregion

        #region UTILITY
        void DisplayCorrectGuessFeedback()
        {
            if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
            _feedbackRoutine = StartCoroutine(DisplayGuessFeedbackRoutine(true));
        }
        void DisplayIncorrectGuessFeedback()
        {
            if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
            _feedbackRoutine = StartCoroutine(DisplayGuessFeedbackRoutine(false));
        }
        #endregion

        #region HELPER METHODS

        void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true)
        {
            textGUI.gameObject.SetActive(canShow);
        }
        IEnumerator DisplayGuessFeedbackRoutine(bool isGuessCorrect)
        {
            string display = string.Empty;
            if (isGuessCorrect)
            {
                if (corerctGuessMessages.Length > 0)
                {
                    int randInt = UnityEngine.Random.Range(0, corerctGuessMessages.Length);
                    display = corerctGuessMessages[randInt];
                    textGUI.color = correctGuessColor;
                }
            }
            else
            {
                if (wrongGuessMessages.Length > 0)
                {
                    int randInt = UnityEngine.Random.Range(0, wrongGuessMessages.Length);
                    display = wrongGuessMessages[randInt];
                    textGUI.color = wrongGuessColor;
                }
            }
            //if (textGUI.gameObject.activeSelf) textGUI.text += string.Concat("\n", display);
            textGUI.text = display;

            //yield return new WaitForSeconds(0.5f);
            ShowTextGUI(textGUI);
            yield return new WaitForSeconds(2f);
            ShowTextGUI(textGUI, false);
            textGUI.text = string.Empty;
        }
        #endregion
    }
}

