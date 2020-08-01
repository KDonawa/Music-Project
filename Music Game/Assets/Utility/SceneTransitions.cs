using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneTransitions : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect = null;
    [SerializeField] CanvasGroup crossfadeCG = null;
    [SerializeField] CanvasGroup circleWipeCG = null;
    [SerializeField] CanvasGroup circleFillCG = null;

    public static float fadeDuration = 2f;

    public const int CROSS_FADE = 1;
    public const int CIRCLE_WIPE_RIGHT = 2;
    public const int CIRCLE_WIPE_LEFT = 3;
    public const int CIRCLE_FILL = 4;

    static SceneTransitions _instance;

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

        crossfadeCG.alpha = 0f;
        _instance.crossfadeCG.interactable = false;

        circleWipeCG.alpha = 0f;
        _instance.circleWipeCG.interactable = false;

        circleFillCG.alpha = 0f;
        _instance.circleFillCG.interactable = false;
    }
    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public static void PlaySceneTransition(int index, Action transitionDelegate)
    {
        switch (index)
        {
            case CROSS_FADE: 
                _instance.StartCoroutine(_instance.CrossFadeRoutine(fadeDuration, transitionDelegate));
                break;
            case CIRCLE_WIPE_RIGHT:
                _instance.StartCoroutine(_instance.CircleWipeRoutine(fadeDuration, transitionDelegate, 1f));
                break;
            case CIRCLE_WIPE_LEFT:
                _instance.StartCoroutine(_instance.CircleWipeRoutine(fadeDuration, transitionDelegate, -1f));
                break;
            case CIRCLE_FILL:
                _instance.StartCoroutine(_instance.CircleFillRoutine(fadeDuration, transitionDelegate));
                break;
            default:
                break;
        }    
    }

    #region FILL
    IEnumerator CircleFillRoutine(float duration, Action transitionDelegate)
    {
        RectTransform circleFillRect = circleFillCG.GetComponent<RectTransform>();

        circleFillRect.localPosition = Vector3.zero;
        circleFillRect.localScale = Vector3.zero;
        _instance.circleFillCG.alpha = 1f;

        float lerp = duration * 0.5f;

        // 1st half
        while (circleFillRect.localScale.x < 1f)
        {
            circleFillRect.localScale += Vector3.one * Time.deltaTime / lerp;
            yield return null;
        }

        transitionDelegate?.Invoke();
        yield return new WaitForSeconds(0.3f);

        // 2nd half
        while (circleFillRect.localScale.x > 0.05f)
        {
            circleFillRect.localScale -= Vector3.one * Time.deltaTime / lerp;
            yield return null;
        }        

        _instance.circleFillCG.blocksRaycasts = false;
        _instance.circleFillCG.alpha = 0f;
    }
    #endregion

    #region WIPE

    IEnumerator CircleWipeRoutine(float duration, Action transitionDelegate, float direction)
    {      
        _instance.circleWipeCG.blocksRaycasts = true;

        RectTransform circleWipeRect = circleWipeCG.GetComponent<RectTransform>();
        float startPos = -canvasRect.rect.width / 2f - circleWipeRect.rect.width / 2f;
        startPos *= direction;
        float endPos = -startPos;

        circleWipeRect.localPosition = Vector3.right * startPos;
        _instance.circleWipeCG.alpha = 1f;

        float lerp = (endPos - startPos) / duration * 2f;

        //first half
        while((0f - circleWipeRect.localPosition.x)*direction > 0f)
        {
            circleWipeRect.localPosition += Vector3.right * lerp * Time.deltaTime;
            yield return null;
        }

        transitionDelegate?.Invoke();
        yield return new WaitForSeconds(0.3f);

        //2nd half
        while ((endPos - circleWipeRect.localPosition.x)*direction > 0f)
        {
            circleWipeRect.localPosition += Vector3.right * lerp * Time.deltaTime;
            yield return null;
        }

        circleWipeRect.localPosition = Vector3.right * endPos;

        _instance.circleWipeCG.blocksRaycasts = false;
        _instance.circleWipeCG.alpha = 0f;
    }
    #endregion

    #region CROSS FADE
    IEnumerator CrossFadeRoutine(float duration, Action transitionDelegate)
    {
        yield return _instance.StartCoroutine(FadeRoutine(0f, 1f, duration*0.5f, true));
        transitionDelegate?.Invoke();
        yield return new WaitForSeconds(0.2f);
        yield return _instance.StartCoroutine(FadeRoutine(1f, 0f, duration*0.5f, false));
    }

    IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration, bool blocksRaycastAfterFade)
    {
        
        _instance.crossfadeCG.blocksRaycasts = true;        

        duration = Mathf.Clamp(duration, 0.1f, Mathf.Infinity);
        _instance.crossfadeCG.alpha = startAlpha;

        float lerp = (endAlpha - startAlpha) / duration;
        while (Mathf.Abs(_instance.crossfadeCG.alpha - endAlpha) > 0f)
        {
            _instance.crossfadeCG.alpha += lerp * Time.deltaTime;            
            yield return null;
        }
        _instance.crossfadeCG.alpha = endAlpha;
        _instance.crossfadeCG.blocksRaycasts = blocksRaycastAfterFade;
    }
    #endregion

}
