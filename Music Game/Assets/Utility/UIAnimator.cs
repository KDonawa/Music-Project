using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

/*
    TODO:
    - MoveTo()
*/
public class UIAnimator : MonoBehaviour
{
    static UIAnimator _instance;

    //Coroutine pulseTextCoroutine;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }            
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    #region UTILITY
    
    /// impact effect. no color change
    public static void ButtonPressEffect1(Button b, string soundEffect)
    {
        if (b == null) return;
        ShrinkAndExpand(b.GetComponent<RectTransform>(), 0.9f, 0.3f);
        AudioManager.PlaySound(soundEffect, SoundType.SFX);
    }
    /// impact effect. Set button text color 
    public static void ButtonPressEffect2(Button b, string soundEffect, Color effectColor)
    {
        if (b == null) return;
        ShrinkAndExpand(b.GetComponent<RectTransform>(), 0.9f, 0.3f);
        SetButtonTextColor(b, effectColor);
        AudioManager.PlaySound(soundEffect, SoundType.SFX);
    }
    /// impact effect. Flash button color white
    public static void ButtonPressEffect3(Button b, string soundEffect)
    {
        ButtonPressEffect4(b, soundEffect, Color.white);
    }
    /// impact effect. Flash button color to desired color
    public static void ButtonPressEffect4(Button b, string soundEffect, Color effectColor)
    {
        if (b == null) return;
        ShrinkAndExpand(b.GetComponent<RectTransform>(), 0.9f, 0.3f);
        FlashButtonColor(b, effectColor, .3f);
        AudioManager.PlaySound(soundEffect, SoundType.SFX);
    }
    #endregion

    #region COLOR
    public static void SetButtonTextColor(Button b, Color color)
    {
        if (b == null) return;
        b.GetComponentInChildren<TextMeshProUGUI>().color = color;
    }
    public static void SetButtonColor(Button b, Color color)
    {
        if (b == null) return;
        b.GetComponent<Image>().color = color;
        b.GetComponentInChildren<TextMeshProUGUI>().color = color;
    }
    public static void FlashButtonColor(Button b, Color color, float duration)
    {
        _instance.StartCoroutine(FlashButtonColorRoutine(b, color, duration));
    }
    public static IEnumerator FlashButtonColorRoutine(Button b, Color color, float duration)
    {
        Image image = b.GetComponent<Image>();
        Color originalColor = image.color;
        image.color = color;

        TextMeshProUGUI textGUI = b.GetComponentInChildren<TextMeshProUGUI>();
        Color originalTextColor = textGUI.color;
        textGUI.color = color;

        duration = Mathf.Clamp(duration, 0.1f, 1.5f);
        float stepSize = duration * 0.5f;
        float lerp = 1 / stepSize;
        yield return new WaitForSeconds(stepSize);

        float timeElapsed = stepSize;
        while (timeElapsed < duration)
        {
            float deltaTime = Time.deltaTime * lerp;
            image.color = Color.Lerp(image.color, originalColor, deltaTime);
            textGUI.color = Color.Lerp(textGUI.color, originalTextColor, deltaTime);
            timeElapsed += deltaTime;
            yield return null;
        }
        if(image) image.color = originalColor;
        if(textGUI)textGUI.color = originalTextColor;
    }
    #endregion

    #region SCALE
    public static void ShrinkAndExpand(RectTransform rect, float percentage, float duration)
    {
        _instance.StartCoroutine(ShrinkAndExpandRoutine(rect, percentage, duration));
    }
    public static IEnumerator ShrinkAndExpandRoutine(RectTransform rect, float fraction, float duration)
    {
        duration = Mathf.Clamp(duration, 0.1f, Mathf.Infinity);
        fraction = Mathf.Clamp01(fraction);

        Vector3 originalScale = new Vector3(1f, 1f, 1f);

        Vector3 scaleDirection = new Vector3(1f, 1f, 0f);
        while (rect.localScale.x > originalScale.x * fraction)
        {
            rect.localScale -= scaleDirection * Time.deltaTime / duration * 0.5f;
            yield return null;
        }
        while (rect.localScale.x < originalScale.x)
        {
            rect.localScale += scaleDirection * Time.deltaTime / duration * 0.5f;
            yield return null;
        }

        rect.localScale = originalScale;
    }

    public static void PulseTextSize(TextMeshProUGUI textGUI, float growthAmount, float period)
    {
        _instance.StartCoroutine(PulseTextSizeRoutine(textGUI, growthAmount, period));
    }
    public static IEnumerator PulseTextSizeRoutine(TextMeshProUGUI textGUI, float growthAmount, float duration)
    {
        Mathf.Clamp(growthAmount, 1f, 5f);
        Mathf.Clamp(duration, 0.1f, Mathf.Infinity);
        float minValue = textGUI.fontSize;
        float maxValue = minValue * growthAmount;

        float lerp = (maxValue - minValue) / duration;
        while (true)
        {
            while (textGUI.fontSize <= maxValue)
            {
                textGUI.fontSize += lerp * Time.deltaTime;
                yield return null;
            }
            while (textGUI.fontSize >= minValue)
            {
                textGUI.fontSize -= lerp * Time.deltaTime;
                yield return null;
            }
            //textGUI.fontSize = minValue;
        }
    }
    public static void ShrinkToNothing(RectTransform rect, float duration, float postEffectDelay, Action onCompletedAction)
    {
        _instance.StartCoroutine(ShrinkToNothingRoutine(rect, duration, postEffectDelay, onCompletedAction));
    }
    public static IEnumerator ShrinkToNothingRoutine(RectTransform rect, float duration, float postEffectDelay, Action EffectCompleted)
    {
        yield return _instance.StartCoroutine(ShrinkToNothingRoutine(rect, duration));

        float remainingTime = postEffectDelay - duration;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        EffectCompleted?.Invoke();
    }
    public static void ShrinkToNothing(RectTransform rect, float duration)
    {
        _instance.StartCoroutine(ShrinkToNothingRoutine(rect, duration));
    }
    public static IEnumerator ShrinkToNothingRoutine(RectTransform rect, float duration)
    {
        duration = Mathf.Abs(duration);
        Vector3 originalScale = Vector3.one;
        Vector3 scaleDirection = new Vector3(1f, 1f, 0f);

        while (rect.localScale.x > 0f)
        {
            rect.localScale -= scaleDirection * Time.deltaTime / duration;
            yield return null;
        }
        rect.gameObject.SetActive(false);
        rect.localScale = originalScale;
    }
    public static void Scale(RectTransform rect, float growthAmount, float duration)
    {
        _instance.StartCoroutine(ScaleRoutine(rect, growthAmount, duration));
    }
    #endregion

    #region ROTATION
    public static void RotateX(RectTransform rect, float amount, float duration)
    {
        _instance.StartCoroutine(RotateRoutine(rect, amount, duration, Vector3.right));      
    }
    public static void RotateY(RectTransform rect, float amount, float duration)
    {
        _instance.StartCoroutine(RotateRoutine(rect, amount, duration, Vector3.up));
    }
    public static void RotateZ(RectTransform rect, float amount, float duration)
    {
        _instance.StartCoroutine(RotateRoutine(rect, amount, duration, Vector3.forward));
    }
    #endregion

    #region TRANSLATION
    public static void MoveX(RectTransform rect, float velocity, float duration)
    {
        _instance.StartCoroutine(MoveRoutine(rect, velocity, duration, Vector3.right*1080));
    }
    public static void MoveY(RectTransform rect, float velocity, float duration)
    {
        _instance.StartCoroutine(MoveRoutine(rect, velocity, duration, Vector3.up * 1920));
    }

    #endregion

    #region HELPERS
    static IEnumerator ScaleRoutine(RectTransform rect, float growthAmount, float duration)
    {
        duration = Mathf.Abs(duration);
        Vector3 originalScale = Vector3.one;
        Vector3 scaleDirection = new Vector3(1f, 1f, 0f);
        Vector3 finalScale = originalScale * growthAmount;

        float sign = Mathf.Sign(growthAmount - 1);

        while ((finalScale.x - rect.localScale.x)*sign > 0)
        {
            rect.localScale += scaleDirection * sign * Time.deltaTime / duration;
            yield return null;
        }
        rect.localScale = finalScale;
        //rect.gameObject.SetActive(false);
        //rect.localScale = originalScale;
    }

    
    static IEnumerator MoveRoutine(RectTransform rect, float velocity, float duration, Vector3 direction)
    {
        duration = Mathf.Abs(duration);
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            rect.localPosition += Time.deltaTime * velocity * direction;
            yield return null;
        }       
    }
    static IEnumerator RotateRoutine(RectTransform rect, float degrees, float duration, Vector3 orientation)
    {
        float sign = Mathf.Sign(degrees);
        degrees = Mathf.Abs(degrees);
        duration = Mathf.Clamp(duration, 0.1f, Mathf.Infinity);

        float currentRotation = 0f;
        while (Mathf.Abs(currentRotation) < degrees)
        {
            currentRotation += Time.deltaTime * degrees/duration * sign;
            rect.localRotation = Quaternion.Euler(orientation * currentRotation);
            yield return null;
        }
        rect.localRotation = Quaternion.Euler(orientation * degrees * sign);
    }
    
    
    #endregion
}
