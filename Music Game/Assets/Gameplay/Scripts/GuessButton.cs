using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GuessButton : MonoBehaviour
{
    string _name;
    Image _image;
    Button _button;
    Color _originalTextColor;
 
    public static event Action<GuessButton> ButtonPressedEvent;
    public static event Action GuessCorrectEvent;
    public static event Action GuessIncorrectEvent;
    public static event Action GuessCheckedEvent;

    public static string correctGuess;

    private void Awake()
    {
        _image = GetComponent<Image>();
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
        CheckGuess();
    }

    public void CheckGuess()
    {
        StartCoroutine(ProcessGuessRoutine(_name == correctGuess));
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

        yield return new WaitForSeconds(0.5f);
        UIAnimator.SetButtonTextColor(_button, _originalTextColor);

        GuessCheckedEvent?.Invoke();
    }

    
}
