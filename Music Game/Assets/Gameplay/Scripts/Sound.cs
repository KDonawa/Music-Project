using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName ="Sound")]
public class Sound : ScriptableObject
{
    public new string name;
    public AudioClip audioClip;
    [Range(0f,1f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;
    public bool canLoop = false;

    [HideInInspector] public AudioSource Source { get; private set; }

    public void InitializeAudioSource(AudioSource source, AudioMixerGroup mixerGroup)
    {
        Source = source; // might need to check if it already has one first
        source.clip = audioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = canLoop;
        source.outputAudioMixerGroup = mixerGroup;
    }

    public void RemoveAudioSource()
    {
        Destroy(Source);
        Source = null;
    }
}
