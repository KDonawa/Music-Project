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
    [SerializeField] Slider slider4 = null;

    [Header("Buttons")]
    [SerializeField] Button backButton = null;

    // save data
    public static Slider Slider1 => Instance.slider1;
    public static Slider Slider2 => Instance.slider2;
    public static Slider Slider3 => Instance.slider3;
    public static Slider Slider4 => Instance.slider4;

    protected override void Awake()
    {
        base.Awake();

        if (backButton == null) Debug.LogError("No reference to Back button");
        else backButton.onClick.AddListener(BackPressed);

        slider1.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.NONE, slider1.value));
        slider2.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.DRONE, slider2.value));
        slider3.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.INSTRUMENT, slider3.value));
        slider4.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.SFX, slider4.value));

        LoadData();
    }
    public override void Open()
    {
        LoadData();
        base.Open();
    }
    public static void ResetSettingsData()
    {
        Instance.slider1.value = 1f;
        Instance.slider2.value = 1f;
        Instance.slider3.value = 1f;
        Instance.slider4.value = 1f;

        BinarySaveSystem.SaveSettings();
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
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            PauseMenu.Instance.Open();
        }
    }

    public static void LoadData()
    {
        SettingsSaveData data = BinarySaveSystem.LoadSettingsData();
        if(data != null)
        {
            Instance.slider1.value = data.value1;
            Instance.slider2.value = data.value2;
            Instance.slider3.value = data.value3;
            Instance.slider4.value = data.value4;
        }
        //SetVolume();
    }
}
