using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    SaveData saveData;
    JsonSaver jsonSaver;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        saveData = new SaveData();
        jsonSaver = new JsonSaver();
    }
    public void Save()
    {
        jsonSaver.Save(saveData);
    }
    public void Load()
    {
        jsonSaver.Load(saveData);
    }
    public float MasterVolume
    {
        get { return saveData.masterVolume; }
        set { saveData.masterVolume = value; }
    }
    public float SFXVolume
    {
        get { return saveData.sfxVolume; }
        set { saveData.sfxVolume = value; }
    }
    public float MusicVolume
    {
        get { return saveData.musicVolume; }
        set { saveData.musicVolume = value; }
    }
    
}
