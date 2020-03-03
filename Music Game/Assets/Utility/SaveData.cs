using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public string hashValue;
    public SaveData()
    {
        masterVolume = 0.5f;
        sfxVolume = 0.5f;
        musicVolume = 0.5f;
        hashValue = string.Empty;
    }
}
