using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KD.MusicGame.Utility.SaveSystem
{
    public static class BinarySaveSystem
    {
        private const string gamestatefilePath = "/gamestatedata";
        private const string settingsfilePath = "/settings";
        private const string gameDatafilePath = "/gamedata";
        private const string filePathEnd = ".sav";

        #region GAME STATE DATA
        public static void SaveGameStateData()
        {
            string path = Application.persistentDataPath + gamestatefilePath + filePathEnd;
            SaveData(path, new GameStateSaveData());
        }
        public static GameStateSaveData LoadGameStateData()
        {
            string path = Application.persistentDataPath + gamestatefilePath + filePathEnd;
            object data = LoadData(path);
            return data != null ? (GameStateSaveData)data : null;
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

        #region GAME DATA
        public static void SaveGameData()
        {
            string path = Application.persistentDataPath + gameDatafilePath + filePathEnd;
            SaveData(path, new GameSaveData());
        }
        public static GameSaveData LoadGameData()
        {
            string path = Application.persistentDataPath + gameDatafilePath + filePathEnd;
            object data = LoadData(path);
            return data != null ? (GameSaveData)data : null;
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
}

