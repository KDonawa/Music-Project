using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public bool[] unlockedStages;
    public int[] numUnlockedLevels;

    public StageData()
    {
        Stage[] stages = GameManager.Instance.GetStages();
        unlockedStages = new bool[stages.Length];
        numUnlockedLevels = new int[stages.Length];
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] != null)
            {
                unlockedStages[i] = stages[i].isUnlocked;
                int count = 0;
                foreach (var level in stages[i].Levels)
                {
                    if (level != null && level.isUnlocked) count++;
                }
                numUnlockedLevels[i] = count;
            }
        }
    }
}
