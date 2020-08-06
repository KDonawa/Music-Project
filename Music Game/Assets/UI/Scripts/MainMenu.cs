using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu<MainMenu>
{
    [Header("References")]
    [SerializeField] Button playButton = null;
    [SerializeField] Button settingsButton = null;
    [SerializeField] Button quitButton = null;

    protected override void Awake()
    {
        base.Awake();
        if (playButton == null) Debug.LogError("No reference to Play button");
        else playButton.onClick.AddListener(OnPlayPressed);

        if (settingsButton == null) Debug.LogError("No reference to Settings button");
        else settingsButton.onClick.AddListener(OnSettingsPressed);

        if (quitButton == null) Debug.LogError("No reference to Quit button");
        else quitButton.onClick.AddListener(OnQuitPressed);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayPressed);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsPressed);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitPressed);
    }

    void OnPlayPressed()
    {
        UIAnimator.ButtonPressEffect3(playButton, AudioManager.buttonChime);     

        UIAnimator.ShrinkToNothing(settingsButton.GetComponent<RectTransform>(), 1f, 2f, 
            () => settingsButton.GetComponent<RectTransform>().gameObject.SetActive(true));
        UIAnimator.ShrinkToNothing(quitButton.GetComponent<RectTransform>(), 1f, 2f,
            () => quitButton.GetComponent<RectTransform>().gameObject.SetActive(true));

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, DroneSelectMenu.Instance.Open);
    }
    void OnSettingsPressed()
    {
        UIAnimator.ButtonPressEffect3(settingsButton, AudioManager.click1);

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, SettingsMenu.Instance.Open);
    }
    void OnQuitPressed()
    {
        UIAnimator.ButtonPressEffect3(quitButton, AudioManager.click1);

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, GameManager.QuitGame);
    }
}
