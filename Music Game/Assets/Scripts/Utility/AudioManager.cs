using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using KD.MusicGame.Gameplay;

namespace KD.MusicGame.Utility
{
    public enum SoundType
    {
        NONE,
        SFX,
        DRONE,
        INSTRUMENT,
    }
    public enum InstrumentType
    {
        HARMONIUM,
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }


        [SerializeField] AudioMixer audioMixer = null;
        [SerializeField] AudioMixerGroup droneMixerGroup = null;
        [SerializeField] AudioMixerGroup instrumentMixerGroup = null;
        [SerializeField] AudioMixerGroup sfxMixerGroup = null;


        [SerializeField] Sound[] uiSounds = null;
        [SerializeField] Sound[] droneSounds = null;
        [SerializeField] Sound[] harmoniumSounds = null;

        [SerializeField] AudioSource backUpInstrumentSource = null;

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


        #region SETUP
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                //audioSources = new List<AudioSource>();
                //sfxMixerGroup.audioMixer.SetFloat();

                foreach (var sound in uiSounds) AddAudioSource(sound, sfxMixerGroup);
                foreach (var sound in droneSounds) AddAudioSource(sound, droneMixerGroup);
                foreach (var sound in harmoniumSounds) AddAudioSource(sound, instrumentMixerGroup);
            }
        }
        void AddAudioSource(Sound sound, AudioMixerGroup mixerGroup)
        {
            if (sound == null) return;
            sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>(), mixerGroup);
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
        public static void SetVolume(SoundType soundType, float volume)
        {
            volume = 20f * Mathf.Log10(volume);
            switch (soundType)
            {
                case SoundType.NONE:
                    Instance.audioMixer.SetFloat("masterVolume", volume);
                    break;
                case SoundType.SFX:
                    Instance.audioMixer.SetFloat("sfxVolume", volume);
                    break;
                case SoundType.DRONE:
                    Instance.audioMixer.SetFloat("droneVolume", volume);
                    break;
                case SoundType.INSTRUMENT:
                    Instance.audioMixer.SetFloat("instrumentVolume", volume);
                    break;
                default:
                    break;
            }
        }
        public static void PlaySound(string name, SoundType soundType, bool canPlay = true)
        {
            Sound sound = Instance.FindSound(name, soundType);
            if (sound != null)
            {

                if (canPlay)
                {
                    if (soundType == SoundType.INSTRUMENT && sound.Source.isPlaying)
                    {
                        Instance.backUpInstrumentSource.clip = sound.Source.clip;
                        Instance.backUpInstrumentSource.Play();
                    }
                    else sound.Source.Play();
                }
                else sound.Source.Stop();
            }
        }
        public static void StopSound(string name, SoundType soundType) => PlaySound(name, soundType, false);
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
                case SoundType.SFX: return Array.Find(uiSounds, s => s.name == name);
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
}

