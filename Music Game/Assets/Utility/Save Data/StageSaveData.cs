namespace KD.MusicGame.Utility.SaveSystem
{
    [System.Serializable]
    public class StageSaveData
    {
        public bool[] unlockedStages;
        public int[] numPassedLevels;

        public StageSaveData()
        {
            Gameplay.Stage[] stages = GameManager.Stages;
            unlockedStages = new bool[stages.Length];
            numPassedLevels = new int[stages.Length];

            for (int i = 0; i < stages.Length; i++)
            {
                if (stages[i] != null)
                {
                    unlockedStages[i] = stages[i].isUnlocked;
                    numPassedLevels[i] = stages[i].numPassedLevels;
                }
            }
        }
    }
}

