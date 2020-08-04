using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GuessButton : MonoBehaviour
{
    string _name;
    Image _image;
    Button _button;

    public static event Action<bool> GuessEvent;
    public event Action GuessRoutineCompletedEvent;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }


    public void Initialize(string name)
    {
        _name = name;
        //_image = GetComponent<Image>();
    }

    public void CheckGuess(string guess, Action correctGuessAction)
    {
        bool isGuessCorrect = guess == _name;
        if (isGuessCorrect) correctGuessAction?.Invoke();
        StartCoroutine(ProcessGuessRoutine(isGuessCorrect));
        //return isGuessCorrect;
    }

    IEnumerator ProcessGuessRoutine(bool isGuessCorrect)
    {     
        if (isGuessCorrect)
        {
            UIAnimator.ButtonPressEffect(_button, AudioManager.correctGuess);
            UIAnimator.SetColor(_image, Color.green);
            //AudioManager.PlaySound(AudioManager.correctGuess, SoundType.UI);
        }
        else
        {
            UIAnimator.ButtonPressEffect(_button, AudioManager.wrongGuess);
            UIAnimator.SetColor(_image, Color.red);
            //AudioManager.PlaySound(AudioManager.wrongGuess, SoundType.UI);
        }
        GuessEvent?.Invoke(isGuessCorrect);

        yield return new WaitForSeconds(0.5f);
        UIAnimator.SetColor(_image, Color.black);

        GuessRoutineCompletedEvent?.Invoke();
    }

    
}
