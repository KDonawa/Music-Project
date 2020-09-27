using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class GuessButton : MonoBehaviour
    {
        string _name;
        Button _button;
        Color _originalTextColor;

        public static event Action<GuessButton> ButtonPressedEvent;
        public static event Action GuessCorrectEvent;
        public static event Action GuessIncorrectEvent;
        public static event Action GuessCheckedEvent;

        public static string correctGuess;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _originalTextColor = _button.GetComponentInChildren<TextMeshProUGUI>().color;
        }

        public void Initialize(string name)
        {
            _name = name;
        }
        public void ButtonPressed()
        {
            ButtonPressedEvent?.Invoke(this);
            StartCoroutine(ProcessGuessRoutine(_name == correctGuess));
        }
        public void ShowCorrectGuess() => StartCoroutine(ShowCorrectGuessRoutine());
        IEnumerator ShowCorrectGuessRoutine()
        {
            if (_name == correctGuess)
            {
                UIAnimator.SetButtonTextColor(_button, Color.green);
                yield return new WaitForSeconds(0.75f);
                UIAnimator.SetButtonTextColor(_button, _originalTextColor);
            }
        }

        IEnumerator ProcessGuessRoutine(bool isGuessCorrect)
        {
            if (isGuessCorrect)
            {
                GuessCorrectEvent?.Invoke();
                UIAnimator.ButtonPressEffect2(_button, AudioManager.correctGuess, Color.green);
            }
            else
            {
                GuessIncorrectEvent?.Invoke();
                UIAnimator.ButtonPressEffect2(_button, AudioManager.wrongGuess, Color.red);
            }

            yield return new WaitForSeconds(0.75f);
            UIAnimator.SetButtonTextColor(_button, _originalTextColor);

            GuessCheckedEvent?.Invoke();
        }
    }
}

