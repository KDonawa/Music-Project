using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KD.MusicGame.Utility.SaveSystem
{
    [System.Serializable]
    public class LevelSaveData
    {
        public bool[] unlockedLevels;
        public bool[] passedLevels;
        public int[] starsEarned;
        public int[] hiScores;

        public LevelSaveData(int stageIndex)
        {
            Gameplay.Level[] levels = GameManager.GetLevelsInStage(stageIndex);
            if (levels != null)
            {
                int size = levels.Length;
                unlockedLevels = new bool[size];
                passedLevels = new bool[size];
                starsEarned = new int[size];                
                hiScores = new int[size];
                for (int i = 0; i < size; i++)
                {
                    if (levels[i] != null)
                    {
                        unlockedLevels[i] = levels[i].isUnlocked;
                        passedLevels[i] = levels[i].isPassed;
                        starsEarned[i] = levels[i].numStarsEarned;
                        hiScores[i] = levels[i].hiScore;
                    }
                }
            }
        }
    }
}

