using KD.MusicGame.Gameplay;
using System.Collections.Generic;

namespace KD.MusicGame.Utility.SaveSystem
{
    [System.Serializable]
    public class GameStateSaveData
    {
        public bool isNewGame = true;

        public GameStateSaveData()
        {
            isNewGame = GameManager.Instance.isNewGame;
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        // stage data
        public int numStages = 0;        
        public string[] stageNames;
        public bool[] unlockedStages;
        public int[] numPassedLevels;

        // level data
        public int[] numLevels;
        public bool[][] unlockedLevels;
        public bool[][] passedLevels;
        public int[][] starsEarned;
        public int[][] hiScores;
        public int[][] numNotesToGuess;

        // sublevel data
        public string[][][][] subLevels;


        public GameSaveData(bool isCustomData)
        {
            List<StageData> stages = isCustomData ? GameManager.Instance.customStagesList : GameManager.Instance.stagesList;
            //List<StageData> stages = GameManager.Instance.stagesList;
            numStages = stages.Count;
            numLevels = new int[numStages];
            stageNames = new string[numStages];
            unlockedStages = new bool[numStages];
            numPassedLevels = new int[numStages];
            unlockedLevels = new bool[numStages][];
            passedLevels = new bool[numStages][];
            starsEarned = new int[numStages][];
            hiScores = new int[numStages][];
            numNotesToGuess = new int[numStages][];
            subLevels = new string[numStages][][][];

            for (int i = 0; i < numStages; i++)
            {
                stageNames[i] = stages[i].name;
                unlockedStages[i] = stages[i].isUnlocked;
                numPassedLevels[i] = stages[i].numPassedLevels;

                numLevels[i] = stages[i].levels.Length;
                unlockedLevels[i] = new bool[numLevels[i]];
                passedLevels[i] = new bool[numLevels[i]];
                starsEarned[i] = new int[numLevels[i]];
                hiScores[i] = new int[numLevels[i]];
                numNotesToGuess[i] = new int[numLevels[i]];
                subLevels[i] = new string[numLevels[i]][][];

                for (int j = 0; j < numLevels[i]; j++)
                {
                    if (stages[i].levels[j] != null)
                    {
                        unlockedLevels[i][j] = stages[i].levels[j].isUnlocked;
                        passedLevels[i][j] = stages[i].levels[j].isPassed;
                        starsEarned[i][j] = stages[i].levels[j].numStarsEarned;
                        hiScores[i][j] = stages[i].levels[j].hiScore;
                        numNotesToGuess[i][j] = stages[i].levels[j].numNotesToGuess;
                        subLevels[i][j] = stages[i].levels[j].subLevels;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class SettingsSaveData
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float noteSpeed;

        public SettingsSaveData()
        {
            value1 = UI.SettingsMenu.Slider1.value;
            value2 = UI.SettingsMenu.Slider2.value;
            value3 = UI.SettingsMenu.Slider3.value;
            value4 = UI.SettingsMenu.Slider4.value;
            noteSpeed = UI.SettingsMenu.NoteSpeedSlider.value;
        }
    }
}
