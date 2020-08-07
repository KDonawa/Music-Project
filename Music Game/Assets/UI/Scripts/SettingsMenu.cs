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
    public float Value1 => slider1.value;
    public float Value2 => slider2.value;
    public float Value3 => slider3.value;

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

        UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);

        SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, MainMenu.Instance.Open);
    }
    void LoadData()
    {
        SettingsData data = BinarySaveSystem.LoadSettingsData();
        if(data != null)
        {
            slider1.value = data.value1;
            slider2.value = data.value2;
            slider3.value = data.value3;
        }
    }
}
