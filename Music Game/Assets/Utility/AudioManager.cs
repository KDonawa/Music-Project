using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Audio;

public enum SoundType
{
    UI,
    DRONE,
    HARMONIUM,
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
    public const string click1 = "click1";
    public const string buttonLoad = "buttonLoad";
    public const string sceneTransition = "sceneTransition";
    public const string buttonChime = "buttonChime";
    public const string timerExpired = "timerExpired";
    public const string chime1 = "chime1";
    public const string win = "win";
    public const string swoosh1 = "swoosh1";
    public const string chime3 = "chime3";

    #region SETUP
    private void Awake()
    {
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // ui sounds
            foreach (var sound in uiSounds) 
            {
                if(sound != null) sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
            }
            // drone sounds
            foreach (var sound in droneSounds)
            {
                if (sound != null) sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
            }
            // harmonium sounds
            foreach (var sound in harmoniumSounds)
            {
                if (sound != null) sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
            }

        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion   
    
    #region UTILITY
    //public void PlaySoundOneShot(string name)
    //{
    //    Sound sound = Array.Find(sounds, s => s.name == name);
    //    if(sound != null)
    //    {
    //        sound.Source.PlayOneShot(sound.audioClip);
    //    }
    //}
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
    public static void StopAllGamplaySounds()
    {
        //StopAllUISounds();
        StopAllDroneSounds();
        StopAllHarmoniumSounds();
    }
    public static void PlaySound(string name, SoundType soundType, bool canPlay = true)
    {
        Sound sound = FindSound(name, soundType);        
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
        Sound sound = FindSound(name, soundType);
        if (sound != null)
        {
            if (isPausing) sound.Source.Pause();
            else sound.Source.UnPause();
        }
    }
    public static void UnPauseSound(string name, SoundType soundType)
    {
        PauseSound(name, soundType, false);
    }

    #endregion

    #region HELPERS
    private static Sound FindSound(string name, SoundType soundType)
    {
        Sound sound = null;
        switch (soundType)
        {
            case SoundType.UI:
                sound = Array.Find(Instance.uiSounds, s => s.name == name);
                break;
            case SoundType.DRONE:
                sound = Array.Find(Instance.droneSounds, s => s.name == name);
                break;
            case SoundType.HARMONIUM:
                sound = Array.Find(Instance.harmoniumSounds, s => s.name == name);
                break;
        }
        return sound;
    }
    #endregion
}
