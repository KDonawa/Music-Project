using UnityEngine;

namespace KD.MusicGame.Gameplay
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
    public class Level : ScriptableObject
    {
        public bool isUnlocked;
        public bool isPassed;
        public int hiScore;
        public int numStarsEarned;
        [Range(1, 10)] public int numNotesToGuess = 1;
        public SubLevel[] subLevels = null;
    }
    public class LevelData
    {
        public bool isUnlocked;
        public bool isPassed;
        public int hiScore;
        public int numStarsEarned;
        public int numNotesToGuess = 1;
        public string[][] subLevels;
    }
}