using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySaveSystem
{
    private const string settingsfilePath = "/settings";
    private const string stageDatafilePath = "/stage";
    private const string levelDatafilePath = "levels";
    private const string filePathEnd = ".sav";

    #region SETTINGS
    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + settingsfilePath + filePathEnd;
        SaveData(path, new SettingsData());
    }
    public static SettingsData LoadSettingsData()
    {       
        string path = Application.persistentDataPath + settingsfilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (SettingsData)data : null;
    }
    #endregion

    #region STAGE DATA
    public static void SaveStageData()
    {
        string path = Application.persistentDataPath + stageDatafilePath + filePathEnd;
        SaveData(path, new StageData());
    }
    public static StageData LoadStageData()
    {
        string path = Application.persistentDataPath + stageDatafilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (StageData)data : null;
    }
    #endregion

    #region LEVEL DATA
    public static void SaveLevelData()
    {
        int stageIndex = GameManager.Instance.CurrentStageIndex;
        string path = Application.persistentDataPath + stageDatafilePath + stageIndex.ToString() + levelDatafilePath + filePathEnd;
        SaveData(path, new LevelData());
    }
    public static LevelData LoadLevelData(int stageIndex)
    {
        string path = Application.persistentDataPath + stageDatafilePath + stageIndex.ToString() + levelDatafilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (LevelData)data : null;
    }
    #endregion

    #region HELPERS
    private static void SaveData(string path, object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }
    private static object LoadData(string path)
    {
        if (File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                object data = formatter.Deserialize(fileStream);
                fileStream.Close();
                return data;
            }
            catch
            {
                Debug.Log("save file not found in " + path);
                fileStream.Close();
                return null;
            }
        }
        return null;
    }
    #endregion
}
