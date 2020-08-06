using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
public class Level : ScriptableObject
{
    //public Sound droneNote = null;
    public SubLevel[] subLevels = null;
    public int numNotesToGuess = 1;
    //public int pointsPerCorrectGuess = 10;
    //[Range(50f, 100f)] public float levelPassPercentage = 75f;
}
