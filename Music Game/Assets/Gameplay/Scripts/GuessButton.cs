using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GuessButton : MonoBehaviour
{
    string _name;
    Image _image;
    Button _button;
 
    public static event Action<GuessButton> ButtonPressedEvent;
    public static event Action GuessCorrectEvent;
    public static event Action GuessIncorrectEvent;
    public static event Action GuessCheckedEvent;

    public static string correctGuess;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void Initialize(string name)
    {
        _name = name;
    }
    public void ButtonPressed()
    {
        ButtonPressedEvent?.Invoke(this);
        CheckGuess(correctGuess);
    }

    public void CheckGuess(string guess)
    {
        StartCoroutine(ProcessGuessRoutine(guess == _name));
    }

    IEnumerator ProcessGuessRoutine(bool isGuessCorrect)
    {     
        if (isGuessCorrect)
        {
            GuessCorrectEvent?.Invoke();
            UIAnimator.ButtonPressEffect4(_button, AudioManager.correctGuess, Color.green);
        }
        else
        {
            GuessIncorrectEvent?.Invoke();
            UIAnimator.ButtonPressEffect4(_button, AudioManager.wrongGuess, Color.red);
        }        

        yield return new WaitForSeconds(2f);

        GuessCheckedEvent?.Invoke();
    }

    
}
