using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class JsonSaver
{
    static readonly string filename = "saveData1.sav";

    public static string GetSaveFilename()
    {
        return Application.persistentDataPath + "/" + filename;
    }

    public void Save(SaveData data)
    {
        data.hashValue = string.Empty; // need consistent reference value

        string jsonString = JsonUtility.ToJson(data);
        data.hashValue = GetSHA256(jsonString);
        jsonString = JsonUtility.ToJson(data);

        string saveFilename = GetSaveFilename();

        FileStream fileStream = new FileStream(saveFilename, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(jsonString);
        }
    }

    public bool Load(SaveData data)
    {
        string loadFileName = GetSaveFilename();
        if (File.Exists(loadFileName))
        {
            using(StreamReader reader = new StreamReader(loadFileName))
            {
                string jsonString = reader.ReadToEnd();

                // check if data has been tampered with before reading
                if (IsSaveDataValid(jsonString))
                {
                    JsonUtility.FromJsonOverwrite(jsonString, data);
                }

                
            }
            return true;
        }
        return false;
    }

    bool IsSaveDataValid(string jsonString)
    {
        SaveData tempSaveData = new SaveData();
        JsonUtility.FromJsonOverwrite(jsonString, tempSaveData);
        string oldHash = tempSaveData.hashValue;

        tempSaveData.hashValue = string.Empty;
        string tempJson = JsonUtility.ToJson(tempSaveData);
        string newHash = GetSHA256(tempJson);

        return newHash == oldHash;
    }

    public void Delete()
    {
        File.Delete(GetSaveFilename());
    }

    public string GetHexStringFromHash(byte[] hash)
    {
        string hexString = string.Empty;

        foreach (byte b in hash)
        {
            hexString += b.ToString("x2");
        }
        return hexString;
    }
    private string GetSHA256(string text)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(text);

        SHA256Managed mySHA256 = new SHA256Managed();

        byte[] hashValue = mySHA256.ComputeHash(textToBytes);

        return GetHexStringFromHash(hashValue);
    }
}
