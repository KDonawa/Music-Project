using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InTransition
{
    FADE_IN,
    CLOSE_VERTICAL,
    CLOSE_HORIZONTAL,
    CIRCLE_EXPAND,
    CIRCLE_WIPE_LEFT,
    CIRCLE_WIPE_RIGHT,
    CIRCLE_WIPE_UP,
    CIRCLE_WIPE_DOWN,
    //SLIDE_HALVES_VERTICALLY_LEFT_UP,
}
public enum OutTransition
{
    FADE_OUT,
    OPEN_VERTICAL,
    OPEN_HORIZONTAL,
    CIRCLE_SHRINK,
    CIRCLE_WIPE_LEFT,
    CIRCLE_WIPE_RIGHT,
    CIRCLE_WIPE_UP,
    CIRCLE_WIPE_DOWN,
}

public class SceneTransitions : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] CanvasGroup mainCG = null;
    [SerializeField] CanvasGroup fadeCG = null;
    //[SerializeField] CanvasGroup circleWipeCG = null;
    [SerializeField] CanvasGroup circleCG = null;
    [SerializeField] CanvasGroup closeOpenCG = null;

    public static float fadeDuration = 2f;

    static SceneTransitions _instance;
    CanvasGroup inTransitonCG;
    WaitForSeconds fadeDelay;

    #region SETUP
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fadeDelay = new WaitForSeconds(0.3f);

    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endregion

    public static void PlayTransition(InTransition inTransition, OutTransition outTransition, Action transitionAction)
    {
        _instance.StartCoroutine(_instance.PlayTransitionRoutine(inTransition, outTransition, transitionAction, fadeDuration));
    }
    IEnumerator PlayTransitionRoutine(InTransition inTransition, OutTransition outTransition, Action transitionAction, float duration)
    {
        mainCG.blocksRaycasts = true;
        duration *= 0.5f;
        inTransitonCG = null;

        // In Transition
        switch (inTransition)
        {
            case InTransition.FADE_IN:
                yield return _instance.StartCoroutine(FadeInRoutine(duration));
                break;
            case InTransition.CLOSE_VERTICAL:                
                yield return _instance.StartCoroutine(CloseRoutine(CloseVertically, duration));
                break;
            case InTransition.CLOSE_HORIZONTAL:
                yield return _instance.StartCoroutine(CloseRoutine(CloseHorizontally, duration));
                break;
            case InTransition.CIRCLE_EXPAND:
                yield return _instance.StartCoroutine(CircleExpandRoutine(duration));
                break;
            case InTransition.CIRCLE_WIPE_LEFT:
                yield return _instance.StartCoroutine(CircleWipeInRoutine(duration, Vector2.left));
                break;
            case InTransition.CIRCLE_WIPE_RIGHT:
                yield return _instance.StartCoroutine(CircleWipeInRoutine(duration, Vector2.right));
                break;
            case InTransition.CIRCLE_WIPE_UP:
                yield return _instance.StartCoroutine(CircleWipeInRoutine(duration, Vector2.up));
                break;
            case InTransition.CIRCLE_WIPE_DOWN:
                yield return _instance.StartCoroutine(CircleWipeInRoutine(duration, Vector2.down));
                break;
            default:
                break;
        }

        if (inTransitonCG == null) yield break;

        transitionAction?.Invoke();
        yield return fadeDelay;

        switch (outTransition)
        {
            case OutTransition.FADE_OUT:
                yield return _instance.StartCoroutine(FadeOutRoutine(duration));
                break;
            case OutTransition.OPEN_VERTICAL:
                yield return _instance.StartCoroutine(OpenRoutine(OpenVertically, duration));
                break;
            case OutTransition.OPEN_HORIZONTAL:
                yield return _instance.StartCoroutine(OpenRoutine(OpenHorizontally, duration));
                break;
            case OutTransition.CIRCLE_SHRINK:
                yield return _instance.StartCoroutine(CircleShrinkRoutine(duration));
                break;
            case OutTransition.CIRCLE_WIPE_LEFT:
                yield return _instance.StartCoroutine(CircleWipeOutRoutine(duration, Vector2.left));
                break;
            case OutTransition.CIRCLE_WIPE_RIGHT:
                yield return _instance.StartCoroutine(CircleWipeOutRoutine(duration, Vector2.right));
                break;
            case OutTransition.CIRCLE_WIPE_UP:
                yield return _instance.StartCoroutine(CircleWipeOutRoutine(duration, Vector2.up));
                break;
            case OutTransition.CIRCLE_WIPE_DOWN:
                yield return _instance.StartCoroutine(CircleWipeOutRoutine(duration, Vector2.down));
                break;
            default:
                break;
        }

        mainCG.blocksRaycasts = false;
    }

    #region IN TRANSITIONS
    IEnumerator FadeInRoutine(float duration)
    {
        inTransitonCG = fadeCG;
        fadeCG.alpha = 0f;

        float lerp = 1 / duration;
        while (fadeCG.alpha < 1f)
        {
            fadeCG.alpha += Time.deltaTime * lerp;
            yield return null;
        }
    }
    IEnumerator CircleExpandRoutine(float duration)
    {
        inTransitonCG = circleCG;
        circleCG.alpha = 0f;        

        RectTransform circleRect = circleCG.GetComponent<RectTransform>();
        circleRect.anchoredPosition = Vector3.zero;
        circleRect.localScale = Vector3.zero;
        float maxDiamater = canvasRect.sizeDelta.magnitude;
        circleRect.sizeDelta = new Vector2(maxDiamater, maxDiamater);

        circleCG.alpha = 1f;

        float lerp = 1 / duration;
        
        while (circleRect.localScale.x < 1f)
        {
            circleRect.localScale += Vector3.one * Time.deltaTime * lerp;
            yield return null;
        }
    }
    IEnumerator CloseRoutine(Func<RectTransform, RectTransform, Vector2> closeDirection, float duration)
    {
        inTransitonCG = closeOpenCG;
        closeOpenCG.alpha = 0f;        

        RectTransform[] rectTransforms = closeOpenCG.GetComponentsInChildren<RectTransform>();
        RectTransform increasing = rectTransforms[1];
        RectTransform decreasing = rectTransforms[2];

        Vector2 direction = closeDirection(increasing, decreasing);

        closeOpenCG.alpha = 1f;

        float dist = Mathf.Abs(Vector2.Dot(direction, increasing.sizeDelta));
        Vector2 lerp = direction * dist / duration;

        float padding = dist * 0.05f; // 5% padding
        while (Vector2.Dot(direction, decreasing.anchoredPosition) > padding)
        {
            Vector2 delta = lerp * Time.deltaTime;
            decreasing.anchoredPosition -= delta;
            increasing.anchoredPosition += delta;
            yield return null;
        }
        decreasing.anchoredPosition = Vector2.zero;
        increasing.anchoredPosition = Vector2.zero;
    }
    IEnumerator CircleWipeInRoutine(float duration, Vector2 direction)
    {
        inTransitonCG = circleCG;
        circleCG.alpha = 0f;

        RectTransform circleRect = circleCG.GetComponent<RectTransform>();
        float diamater = canvasRect.sizeDelta.magnitude;
        Vector2 initialPosition = -0.5f * (direction * diamater + direction * canvasRect.sizeDelta);

        circleRect.localScale = Vector3.one;        
        circleRect.sizeDelta = new Vector2(diamater, diamater);
        circleRect.anchoredPosition = initialPosition;

        circleCG.alpha = 1f;

        Vector2 lerp = direction * Mathf.Abs(Vector2.Dot(initialPosition, direction)) / duration;
        while(Vector2.Dot(circleRect.anchoredPosition, direction) < 0f)
        {
            circleRect.anchoredPosition += lerp * Time.deltaTime;
            yield return null;
        }
    }
    
    #endregion

    #region OUT TRANSITIONS
    IEnumerator FadeOutRoutine(float duration)
    {
        if (inTransitonCG != null) inTransitonCG.alpha = 0f;
        fadeCG.alpha = 1f;

        float lerp = 1 / duration;
        while (fadeCG.alpha > 0f)
        {
            fadeCG.alpha -= Time.deltaTime * lerp;
            yield return null;
        }
        _instance.fadeCG.alpha = 0f;
    }
    IEnumerator CircleShrinkRoutine(float duration)
    {
        RectTransform circleRect = circleCG.GetComponent<RectTransform>();
        circleRect.anchoredPosition = Vector3.zero;
        circleRect.localScale = Vector3.one;
        float maxDiamater = canvasRect.sizeDelta.magnitude;
        circleRect.sizeDelta = new Vector2(maxDiamater, maxDiamater);

        if(inTransitonCG != null) inTransitonCG.alpha = 0f;
        circleCG.alpha = 1f;

        float lerp = 1 / duration;
        while (circleRect.localScale.x > 0.1f)
        {
            circleRect.localScale -= Vector3.one * Time.deltaTime * lerp;
            yield return null;
        }
        circleCG.alpha = 0f;
    }
    IEnumerator OpenRoutine(Func<RectTransform, RectTransform, Vector2> openDirection, float duration)
    {
        RectTransform[] rectTransforms = closeOpenCG.GetComponentsInChildren<RectTransform>();
        RectTransform increasing = rectTransforms[2];
        RectTransform decreasing = rectTransforms[1];

        Vector2 direction = openDirection(increasing, decreasing);

        if (inTransitonCG != null) inTransitonCG.alpha = 0f;
        closeOpenCG.alpha = 1f;

        float dist = Mathf.Abs(Vector2.Dot(direction, increasing.sizeDelta));
        Vector2 lerp = direction * dist / duration;
        while (Vector2.Dot(direction, increasing.anchoredPosition) < dist)
        {
            Vector2 delta = lerp * Time.deltaTime;
            decreasing.anchoredPosition -= delta;
            increasing.anchoredPosition += delta;
            yield return null;
        }
        closeOpenCG.alpha = 0f;
    }
    IEnumerator CircleWipeOutRoutine(float duration, Vector2 direction)
    {
        RectTransform circleRect = circleCG.GetComponent<RectTransform>();
        float diamater = canvasRect.sizeDelta.magnitude;
        Vector2 finalPosition = 0.5f * (direction * diamater + direction * canvasRect.sizeDelta);

        circleRect.localScale = Vector3.one;
        circleRect.sizeDelta = new Vector2(diamater, diamater);
        circleRect.anchoredPosition = Vector2.zero;

        if (inTransitonCG != null) inTransitonCG.alpha = 0f;
        circleCG.alpha = 1f;

        Vector2 lerp = direction * Mathf.Abs(Vector2.Dot(finalPosition, direction)) / duration;
        while (Vector2.Dot(finalPosition - circleRect.anchoredPosition, direction) > 0f)
        {
            circleRect.anchoredPosition += lerp * Time.deltaTime;
            yield return null;
        }

        circleCG.alpha = 0f;
    }
    #endregion


    #region HELPERS

    Vector2 CloseVertically(RectTransform increasing, RectTransform decreasing)
    {
        Vector2 size = new Vector2(canvasRect.rect.width, canvasRect.rect.height / 2f);

        decreasing.sizeDelta = size;
        decreasing.pivot = new Vector2(0.5f, 1f);
        decreasing.anchorMin = new Vector2(0.5f, 1f);
        decreasing.anchorMax = new Vector2(0.5f, 1f);
        decreasing.anchoredPosition = new Vector2(0f, size.y);

        increasing.sizeDelta = size;
        increasing.pivot = new Vector2(0.5f, 0f);
        increasing.anchorMin = new Vector2(0.5f, 0f);
        increasing.anchorMax = new Vector2(0.5f, 0f);
        increasing.anchoredPosition = new Vector2(0f, -size.y);

        return Vector2.up;
    }
    Vector2 CloseHorizontally(RectTransform increasing, RectTransform decreasing)
    {
        Vector2 size = new Vector2(canvasRect.rect.width / 2f, canvasRect.rect.height);

        increasing.sizeDelta = size;
        increasing.pivot = new Vector2(0f, 0.5f);
        increasing.anchorMin = new Vector2(0f, 0.5f);
        increasing.anchorMax = new Vector2(0f, 0.5f);
        increasing.anchoredPosition = new Vector2(-size.x, 0f);

        decreasing.sizeDelta = size;
        decreasing.pivot = new Vector2(1f, 0.5f);
        decreasing.anchorMin = new Vector2(1f, 0.5f);
        decreasing.anchorMax = new Vector2(1f, 0.5f);
        decreasing.anchoredPosition = new Vector2(size.x, 0f);

        return Vector2.right;
    }
    Vector2 OpenVertically(RectTransform increasing, RectTransform decreasing)
    {
        Vector2 size = new Vector2(canvasRect.rect.width, canvasRect.rect.height / 2f);

        increasing.sizeDelta = size;
        increasing.pivot = new Vector2(0.5f, 1f);
        increasing.anchorMin = new Vector2(0.5f, 1f);
        increasing.anchorMax = new Vector2(0.5f, 1f);
        increasing.anchoredPosition = Vector2.zero;

        decreasing.sizeDelta = size;
        decreasing.pivot = new Vector2(0.5f, 0f);
        decreasing.anchorMin = new Vector2(0.5f, 0f);
        decreasing.anchorMax = new Vector2(0.5f, 0f);
        decreasing.anchoredPosition = Vector2.zero;

        return Vector2.up;
    }
    Vector2 OpenHorizontally(RectTransform increasing, RectTransform decreasing)
    {
        Vector2 size = new Vector2(canvasRect.rect.width / 2f, canvasRect.rect.height);

        decreasing.sizeDelta = size;
        decreasing.pivot = new Vector2(0f, 0.5f);
        decreasing.anchorMin = new Vector2(0f, 0.5f);
        decreasing.anchorMax = new Vector2(0f, 0.5f);
        decreasing.anchoredPosition = Vector2.zero;

        increasing.sizeDelta = size;
        increasing.pivot = new Vector2(1f, 0.5f);
        increasing.anchorMin = new Vector2(1f, 0.5f);
        increasing.anchorMax = new Vector2(1f, 0.5f);
        increasing.anchoredPosition = Vector2.zero;

        return Vector2.right;
    }    
    
    #endregion


}
