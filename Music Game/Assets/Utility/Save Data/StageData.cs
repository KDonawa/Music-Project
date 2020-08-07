using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public bool[] unlockedStages;   

    public StageData()
    {
        Stage[] stages = GameManager.Instance.GetStages();
        unlockedStages = new bool[stages.Length];
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] != null) unlockedStages[i] = stages[i].isUnlocked;
        }
    }
}
