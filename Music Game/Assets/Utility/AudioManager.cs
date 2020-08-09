using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Audio;

public enum SoundType
{
    UI,
    DRONE,
    INSTRUMENT,
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] Sound[] uiSounds = null;
    [SerializeField] Sound[] droneSounds = null;
    [SerializeField] Sound[] harmoniumSounds = null;

    // UI Sounds
    public const string countdown = "countdown";
    public const string timer = "timer";
    public const string correctGuess = "correctGuess";
    public const string wrongGuess = "wrongGuess";
    public const string buttonSelect2 = "buttonSelect2";
    public const string buttonLoad1 = "buttonLoad1";
    public const string buttonSelect1 = "buttonSelect1";
    public const string timerExpired = "timerExpired";
    public const string starDisplay = "starDisplay";
    public const string badgeDisplay = "badgeDisplay";
    public const string stageComplete = "stageComplete";
    public const string swoosh1 = "swoosh1";
    public const string success = "success";
    public const string finalScoreUpdate = "finalScoreUpdate";

    //List<AudioSource> audioSources;

    #region SETUP
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //audioSources = new List<AudioSource>();

            foreach (var sound in uiSounds) AddAudioSource(sound);
            foreach (var sound in droneSounds) AddAudioSource(sound);
            foreach (var sound in harmoniumSounds) AddAudioSource(sound);
        }
    }
    void AddAudioSource(Sound sound)
    {
        if (sound == null) return;
        //AudioSource source = new AudioSource();
        //audioSources.Add(source);
        //sound.InitializeAudioSource(source);
        sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            //audioSources.Clear();
        }
    }
    #endregion   
    
    #region UTILITY
    public static void PlaySound(string name, SoundType soundType, bool canPlay = true)
    {
        Sound sound = Instance.FindSound(name, soundType);        
        if (sound != null)
        {
            if(canPlay) sound.Source.Play();
            else sound.Source.Stop();
        }
    }
    public static void StopSound(string name, SoundType soundType)
    {
        PlaySound(name, soundType, false);
    }
    public static void PauseSound(string name, SoundType soundType, bool isPausing = true)
    {
        Sound sound = Instance.FindSound(name, soundType);
        if (sound != null)
        {
            if (isPausing) sound.Source.Pause();
            else sound.Source.UnPause();
        }
    }    
    public static void UnPauseSound(string name, SoundType soundType) => PauseSound(name, soundType, false);
    public static void PauseSounds(List<string> sounds, SoundType soundType, bool isPausing = true)
    {
        foreach (var soundName in sounds) PauseSound(soundName, soundType, isPausing);
    }
    public static void UnPuaseSounds(List<string> sounds, SoundType soundType) => PauseSounds(sounds, soundType, false);
    public static void StopAllUISounds()
    {
        foreach (var sound in Instance.uiSounds)
        {
            if (sound != null) sound.Source.Stop();
        }
    }
    public static void StopAllDroneSounds()
    {
        foreach (var sound in Instance.droneSounds)
        {
            if (sound != null) sound.Source.Stop();
        }
    }
    public static void StopAllHarmoniumSounds()
    {
        foreach (var sound in Instance.harmoniumSounds)
        {
            if (sound != null) sound.Source.Stop();
        }
    }
    public static void StopAllNoteSounds()
    {
        StopAllDroneSounds();
        StopAllHarmoniumSounds();
    }

    #endregion

    #region HELPERS
    private Sound FindSound(string name, SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.UI: return Array.Find(uiSounds, s => s.name == name);
            case SoundType.DRONE: return Array.Find(droneSounds, s => s.name == name);
            case SoundType.INSTRUMENT: return FindInstrumentSound(name);
            default: return null;
        }
    }
    private Sound FindInstrumentSound(string name)
    {
        switch (GameManager.Instrument)
        {
            case InstrumentType.HARMONIUM: return Array.Find(harmoniumSounds, s => s.name == name);
            default: return null;
        }
    }
    #endregion
}
