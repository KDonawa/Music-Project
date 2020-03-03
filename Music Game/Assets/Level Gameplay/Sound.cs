using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sound")]
public class Sound : ScriptableObject
{
    public new string name;
    public AudioClip audioClip;
    [Range(0f,1f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;
    public bool canLoop = false;

    [HideInInspector] public AudioSource Source { get; private set; }

    public void InitializeAudioSource(AudioSource source)
    {
        Source = source;
        source.clip = audioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = canLoop;
    }
}
