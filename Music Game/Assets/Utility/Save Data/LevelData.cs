using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public bool[] unlockedLevels;
    public int[] starsEarned;

    public LevelData()
    {
        Level[] levels = GameManager.Instance.GetLevelsInCurrentStage();
        if (levels != null)
        {
            unlockedLevels = new bool[levels.Length];
            starsEarned = new int[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                if(levels[i] != null)
                {
                    unlockedLevels[i] = levels[i].isUnlocked;
                    starsEarned[i] = levels[i].numStarsEarned;
                } 
            }
        }
    }
}
