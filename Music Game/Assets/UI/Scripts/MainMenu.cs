using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu<MainMenu>
{
    [Header("References")]
    [SerializeField] Button newGameButton = null;
    [SerializeField] Button continueButton = null;
    [SerializeField] Button settingsButton = null;
    [SerializeField] Button quitButton = null;
    [SerializeField] GameObject newGameTooltip = null;

    protected override void Awake()
    {
        base.Awake();        

        newGameButton.onClick.AddListener(NewGamePressed);
        continueButton.onClick.AddListener(ContinueGamePressed);
        settingsButton.onClick.AddListener(OnSettingsPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        newGameButton.onClick.RemoveListener(NewGamePressed);
        continueButton.onClick.RemoveListener(ContinueGamePressed);
        settingsButton.onClick.RemoveListener(OnSettingsPressed);
        quitButton.onClick.RemoveListener(OnQuitPressed);
    }

    public override void Open()
    {
        continueButton.gameObject.SetActive(!GameManager.Instance.isNewGame);
        newGameTooltip.gameObject.SetActive(false);
        base.Open();            
    }

    #region BUTTON EVENTS
    void ContinueGamePressed()
    {
        UIAnimator.ButtonPressEffect3(continueButton, AudioManager.buttonSelect1);
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, DroneSelectMenu.Instance.Open);
    }

    void NewGamePressed()
    {
        UIAnimator.ButtonPressEffect3(newGameButton, AudioManager.buttonSelect1);
        if (GameManager.Instance.isNewGame)
        {
            GameManager.StartNewGame();

            //UIAnimator.ShrinkToNothing(settingsButton.GetComponent<RectTransform>(), 1f, 2f, 
            //    () => settingsButton.GetComponent<RectTransform>().gameObject.SetActive(true));
            //UIAnimator.ShrinkToNothing(quitButton.GetComponent<RectTransform>(), 1f, 2f,
            //    () => quitButton.GetComponent<RectTransform>().gameObject.SetActive(true));

            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, DroneSelectMenu.Instance.Open);
        }
        else newGameTooltip.gameObject.SetActive(true);
    }    

    void OnSettingsPressed()
    {
        UIAnimator.ButtonPressEffect3(settingsButton, AudioManager.buttonSelect2);

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, SettingsMenu.Instance.Open);
    }
    void OnQuitPressed()
    {
        UIAnimator.ButtonPressEffect3(quitButton, AudioManager.buttonSelect2);

        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, GameManager.QuitGame);
    }
    #endregion
}
