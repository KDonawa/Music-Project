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
        public string name = "Stage";
        public bool isUnlocked = false;
        public int numPassedLevels = 0;
        public LevelData[] levels;
    }
}