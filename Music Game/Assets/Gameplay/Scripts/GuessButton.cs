﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GuessButton : MonoBehaviour
{
    string _name;
    Image _image;
    Button _button;
 
    public static event Action<GuessButton> GuessEvent;
    public static event Action GuessCorrectEvent;
    public static event Action GuessIncorrectEvent;
    public event Action GuessRoutineCompletedEvent;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    public void Initialize(string name)
    {
        _name = name;
    }
    public void ButtonPressed() => GuessEvent?.Invoke(this);

    public void ProcessGuess(string guess)
    {
        StartCoroutine(ProcessGuessRoutine(guess == _name));
    }

    IEnumerator ProcessGuessRoutine(bool isGuessCorrect)
    {     
        if (isGuessCorrect)
        {
            GuessCorrectEvent?.Invoke();
            UIAnimator.ButtonPressEffect(_button, AudioManager.correctGuess, Color.green);
        }
        else
        {
            GuessIncorrectEvent?.Invoke();
            UIAnimator.ButtonPressEffect(_button, AudioManager.wrongGuess, Color.red);
        }        

        yield return new WaitForSeconds(0.5f);

        GuessRoutineCompletedEvent?.Invoke();
    }

    
}