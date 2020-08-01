using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuessButton : MonoBehaviour
{
    string _name;
    Image _image;

    public static event System.Action<bool> GuessEvent;
    public event System.Action GuessRoutineCompletedEvent;

    public void Initialize(string name)
    {
        _name = name;
        _image = GetComponent<Image>();
    }

    public bool CheckGuess(string guess)
    {
        bool isGuessCorrect = guess == _name;
        StartCoroutine(ProcessGuessRoutine(isGuessCorrect));
        return isGuessCorrect;
    }

    IEnumerator ProcessGuessRoutine(bool isGuessCorrect)
    {
        GuessEvent?.Invoke(isGuessCorrect);
        if (isGuessCorrect)
        {
            UIAnimator.SetColor(_image, Color.green);
            AudioManager.Instance.PlaySound("correct guess");
        }
        else
        {
            UIAnimator.SetColor(_image, Color.red);
            AudioManager.Instance.PlaySound("wrong guess");
        }
        yield return new WaitForSeconds(0.5f);
        UIAnimator.SetColor(_image, Color.black);

        GuessRoutineCompletedEvent?.Invoke();
    }

    
}
