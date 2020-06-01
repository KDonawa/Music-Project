using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MenuGeneric<SettingsMenu>
{
    [SerializeField] Slider masterVolumeSlider = null;
    [SerializeField] Slider sfxVolumeSlider = null;
    [SerializeField] Slider musicVolumeSlider = null;

    DataManager dataManager;
    protected override void Awake()
    {
        base.Awake();
        dataManager = FindObjectOfType<DataManager>();
    }
    private void Start()
    {
        LoadData();
    }
    public override void OnBackPressed()
    {
        base.OnBackPressed();
        if(dataManager) dataManager.Save();
    }
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
