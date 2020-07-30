using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ScreenFader))]
public class SplashScreen : MonoBehaviour
{  
    [SerializeField] float delay = 1f;

    ScreenFader screenFader;
    bool isFading = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        screenFader = GetComponent<ScreenFader>();
    }

    private void Start()
    {
        screenFader.FadeOn();
    }
    public void FadeAndLoad()
    {
        if (isFading) return;
        StartCoroutine(FadeAndLoadRoutine());
    }
    IEnumerator FadeAndLoadRoutine()
    {
        isFading = true;

        yield return new WaitForSeconds(delay);
        screenFader.FadeOff(); // fade splash screen

        GameManager.LoadStartScene();

        yield return new WaitForSeconds(screenFader.FadeOffDuration);
        isFading = false;
        Destroy(gameObject); // remove splash screen after fade is complete
    }
}
