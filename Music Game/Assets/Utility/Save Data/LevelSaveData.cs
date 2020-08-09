using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public bool[] unlockedLevels;
    public bool[] passedLevels;
    public int[] starsEarned;

    public LevelSaveData(int stageIndex)
    {
        Level[] levels = GameManager.GetLevelsInStage(stageIndex);
        if (levels != null)
        {
            unlockedLevels = new bool[levels.Length];
            starsEarned = new int[levels.Length];
            passedLevels = new bool[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                if(levels[i] != null)
                {
                    unlockedLevels[i] = levels[i].isUnlocked;
                    passedLevels[i] = levels[i].isPassed;
                    starsEarned[i] = levels[i].numStarsEarned;
                } 
            }
        }
    }
}
