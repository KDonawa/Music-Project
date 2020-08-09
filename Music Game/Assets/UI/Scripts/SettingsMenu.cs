using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu<SettingsMenu>
{
    [Header("Volume Sliders")]
    [SerializeField] Slider slider1 = null;
    [SerializeField] Slider slider2 = null;
    [SerializeField] Slider slider3 = null;

    [Header("Buttons")]
    [SerializeField] Button backButton = null;

    // save data
    public static Slider Slider1 => Instance.slider1;
    public static Slider Slider2 => Instance.slider2;
    public static Slider Slider3 => Instance.slider3;

    protected override void Awake()
    {
        base.Awake();

        if (backButton == null) Debug.LogError("No reference to Back button");
        else backButton.onClick.AddListener(BackPressed);
    }
    private void Start()
    {
        LoadData();        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (backButton != null) backButton.onClick.RemoveListener(BackPressed);
    }

    void BackPressed()
    {
        BinarySaveSystem.SaveSettings();

        

        ReturnToPreviousMenu();
    }
    void ReturnToPreviousMenu()
    {
        if (GameManager.GetCurrentSceneName() == GameManager.StartScene)
        {
            UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, MainMenu.Instance.Open);
        }
        else if(GameManager.GetCurrentSceneName() == GameManager.GameScene)
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.UI);
            PauseMenu.Instance.Open();
        }
    }
    void LoadData()
    {
        SettingsSaveData data = BinarySaveSystem.LoadSettingsData();
        if(data != null)
        {
            slider1.value = data.value1;
            slider2.value = data.value2;
            slider3.value = data.value3;
        }
    }
}
