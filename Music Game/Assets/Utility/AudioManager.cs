using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    //[SerializeField] Sound[] sounds = null;

    Dictionary<string, AudioSource> soundDictionary = new Dictionary<string, AudioSource>();

    #region SETUP
    private void Awake()
    {
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    public static void AddSound(Sound sound)
    {
        if (sound == null) return;
        if (!Instance.soundDictionary.ContainsKey(sound.name))
        {
            sound.InitializeAudioSource(Instance.gameObject.AddComponent<AudioSource>());
            Instance.soundDictionary.Add(sound.name, sound.Source);
        }
    }
    public static void PlaySoundOneShot(string name)
    {
        AudioSource source;
        if (Instance.soundDictionary.TryGetValue(name, out source))
        {
            source.PlayOneShot(source.clip);
        }
    }
    public static void PlaySound(string name, bool canPlay = true)
    {
        AudioSource source;
        if (Instance.soundDictionary.TryGetValue(name, out source))
        {
            if (canPlay) { source.Play(); }
            else { source.Stop(); }
        }
    }
    public static void StopSound(string name)
    {
        PlaySound(name, false);
    }
    public static void PauseSound(string name, bool isPausing = true)
    {
        AudioSource source;
        if (Instance.soundDictionary.TryGetValue(name, out source))
        {
            if (isPausing)
            {
                if (!source.isPlaying) source.Pause();
            }
            else { source.UnPause(); }
        }
    }
    public static void ResumeSound(string name)
    {
        PauseSound(name, false);
    }
    #endregion

}
