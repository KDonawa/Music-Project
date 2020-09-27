using UnityEngine;

namespace KD.MusicGame.Gameplay
{
    [CreateAssetMenu(menuName = "Game/Stage")]
    public class Stage : ScriptableObject
    {
        public new string name = "New Stage";
        public bool isUnlocked;
        public int numPassedLevels = 0;
        [SerializeField] Level[] levels = null;

        public Level[] Levels => levels;
    }
    public class StageData
    {
        public string name = "New Stage";
        public bool isUnlocked;
        public int numPassedLevels;
        public LevelData[] levels;
    }

    [CreateAssetMenu(menuName = "Game/Custom Stage")]
    public class CustomStage : ScriptableObject
    {
        public new string name = "Custom Stage";
        public bool isActive;
        public int numActiveLevels;
        public int numPassedLevels;
        [SerializeField] Level[] levels = null;
        public Level[] Levels => levels;
    }
}