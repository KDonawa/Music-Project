using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] Sound[] sounds = null;

    //Dictionary<string, AudioSource> soundDictionary = new Dictionary<string, AudioSource>();
    List<string> notesList = new List<string>();

    #region SETUP
    private void Awake()
    {
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            foreach (var sound in sounds) 
            {
                if(sound != null)
                {
                    sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
                }
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

    #region HELPERS

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
    public void StopAllSounds()
    {
        foreach (var sound in sounds)
        {
            if(sound != null)
            {
                sound.Source.Stop();
            }
        }
    }
    public void PlaySound(string name, bool canPlay = true)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound != null)
        {
            if(canPlay) sound.Source.Play();
            else sound.Source.Stop();
        }
    }
    public void StopSound(string name)
    {
        PlaySound(name, false);
    }
    public void PauseSound(string name, bool isPausing = true)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound != null)
        {
            if (isPausing) sound.Source.Pause();
            else sound.Source.UnPause();
        }
    }
    public void UnPauseSound(string name)
    {
        PauseSound(name, false);
    }
    
    #endregion

}
