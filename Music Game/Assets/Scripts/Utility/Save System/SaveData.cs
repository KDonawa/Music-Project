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
        public string[] stageNames;
        public bool[] unlockedStages;
        public int[] numPassedLevels;

        // level data
        public bool[] unlockedLevels;
        public bool[] passedLevels;
        public int[] starsEarned;
        public int[] hiScores;
        public int[] numNotesToGuess;

        // sublevel data
        public string[][][] subLevels;

        public GameSaveData()
        {
            List<StageData> stages = GameManager.Instance.stagesList;
            int numStages = stages.Count;
            stageNames = new string[numStages];
            unlockedStages = new bool[numStages];
            numPassedLevels = new int[numStages];

            for (int i = 0; i < numStages; i++)
            {
                stageNames[i] = stages[i].name;
                unlockedStages[i] = stages[i].isUnlocked;
                numPassedLevels[i] = stages[i].numPassedLevels;

                int numLevels = stages[i].levels.Length;
                unlockedLevels = new bool[numLevels];
                passedLevels = new bool[numLevels];
                starsEarned = new int[numLevels];
                hiScores = new int[numLevels];
                numNotesToGuess = new int[numLevels];
                subLevels = new string[numLevels][][];

                for (int j = 0; j < numLevels; j++)
                {
                    if (stages[i].levels[j] != null)
                    {
                        unlockedLevels[j] = stages[i].levels[j].isUnlocked;
                        passedLevels[j] = stages[i].levels[j].isPassed;
                        starsEarned[j] = stages[i].levels[j].numStarsEarned;
                        hiScores[j] = stages[i].levels[j].hiScore;
                        numNotesToGuess[j] = stages[i].levels[j].numNotesToGuess;
                        subLevels[j] = stages[i].levels[j].subLevels;
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
