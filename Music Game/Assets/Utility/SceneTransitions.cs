using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneTransitions : MonoBehaviour
{
    [SerializeField] CanvasGroup crossfadeCG = null;

    public static float fadeDuration = 2f;

    public static int CROSS_FADE = 0;  

    static SceneTransitions _instance;

    //delegate void TransitionDelegate();
    //TransitionDelegate transitionDelegate;
    //static event Action transitionDelegate;
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
            case 0: _instance.StartCoroutine(CrossFadeRoutine(fadeDuration, transitionDelegate));
                break;
            default:
                break;
        }    
    }
    static IEnumerator CrossFadeRoutine(float duration, Action transitionDelegate)
    {
        yield return _instance.StartCoroutine(FadeRoutine(0f, 1f, duration*0.5f, true));
        transitionDelegate?.Invoke();
        yield return new WaitForSeconds(0.2f);
        yield return _instance.StartCoroutine(FadeRoutine(1f, 0f, duration*0.5f, false));
    }

    static IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration, bool blocksRaycastAfterFade)
    {
        
        _instance.crossfadeCG.blocksRaycasts = true;
        _instance.crossfadeCG.interactable = false;

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
        //_instance.crossfadeCG.interactable = true;
    }

}
