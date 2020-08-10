using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class BinarySaveSystem
{
    private const string gamefilePath = "/game";
    private const string settingsfilePath = "/settings";
    private const string stageDatafilePath = "/stage";
    private const string levelDatafilePath = "levels";
    private const string filePathEnd = ".sav";

    #region GAME DATA
    public static void SaveGameData()
    {
        string path = Application.persistentDataPath + gamefilePath + filePathEnd;
        SaveData(path, new GameSaveData());
    }
    public static GameSaveData LoadGameData()
    {
        string path = Application.persistentDataPath + gamefilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (GameSaveData)data : null;
    }
    #endregion

    #region SETTINGS
    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + settingsfilePath + filePathEnd;
        SaveData(path, new SettingsSaveData());
    }
    public static SettingsSaveData LoadSettingsData()
    {       
        string path = Application.persistentDataPath + settingsfilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (SettingsSaveData)data : null;
    }
    #endregion

    #region STAGE DATA
    public static void SaveStageData()
    {
        string path = Application.persistentDataPath + stageDatafilePath + filePathEnd;
        SaveData(path, new StageSaveData());
    }
    public static StageSaveData LoadStageData()
    {
        string path = Application.persistentDataPath + stageDatafilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (StageSaveData)data : null;
    }
    #endregion

    #region LEVEL DATA
    public static void SaveLevelData(int stageIndex)
    {
        string path = Application.persistentDataPath + stageDatafilePath + stageIndex.ToString() + levelDatafilePath + filePathEnd;
        SaveData(path, new LevelSaveData(stageIndex));
    }
    public static LevelSaveData LoadLevelData(int stageIndex)
    {
        string path = Application.persistentDataPath + stageDatafilePath + stageIndex.ToString() + levelDatafilePath + filePathEnd;
        object data = LoadData(path);
        return data != null ? (LevelSaveData)data : null;
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
