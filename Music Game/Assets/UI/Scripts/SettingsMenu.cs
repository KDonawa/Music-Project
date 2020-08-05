﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : /*MenuGeneric<SettingsMenu>*/Menu<SettingsMenu>
{
    [Header("Volume Sliders")]
    [SerializeField] Slider masterVolumeSlider = null;
    [SerializeField] Slider sfxVolumeSlider = null;
    [SerializeField] Slider musicVolumeSlider = null;

    [Header("Buttons")]
    [SerializeField] Button backButton = null;

    DataManager dataManager;
    protected override void Awake()
    {
        base.Awake();
        dataManager = FindObjectOfType<DataManager>();
    }
    private void Start()
    {
        LoadData();

        if (backButton == null) Debug.LogError("No reference to Back button");
        else backButton.onClick.AddListener(OnMainMenuPressed);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (backButton != null) backButton.onClick.RemoveListener(OnMainMenuPressed);
    }

    void OnMainMenuPressed()
    {
        UIAnimator.ButtonPressEffect(backButton, AudioManager.click1);

        SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, MainMenu.Open);
        if (dataManager) dataManager.Save();
    }
    //public override void OnBackPressed()
    //{
    //    UIAnimator.ButtonPressEffect(backButton, AudioManager.click1);

    //    SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, base.OnBackPressed);
    //    if (dataManager) dataManager.Save();
    //}
    public void OnMasterVolumeChanged(float volume)
    {
        if (dataManager)
        {
            dataManager.MasterVolume = volume;
        }
    }
    public void OnSFXVolumeChanged(float volume)
    {
        if (dataManager)
        {
            dataManager.SFXVolume = volume;
        }
    }
    public void OnMusicVolumeChanged(float volume)
    {
        if (dataManager)
        {
            dataManager.MusicVolume = volume;
        }
    }
    void LoadData()
    {
        if(!dataManager || !masterVolumeSlider || !sfxVolumeSlider || !musicVolumeSlider)
        {
            return;
        }
        dataManager.Load();

        masterVolumeSlider.value = dataManager.MasterVolume;
        sfxVolumeSlider.value = dataManager.SFXVolume;
        musicVolumeSlider.value = dataManager.MusicVolume;
    }
}