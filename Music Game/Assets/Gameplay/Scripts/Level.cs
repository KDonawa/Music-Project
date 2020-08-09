using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
public class Level : ScriptableObject
{
    public bool isUnlocked;
    public bool isPassed;
    public int numStarsEarned;
    [Range(1, 10)] public int numNotesToGuess = 1;
    public SubLevel[] subLevels = null;
    //[Range(50f, 100f)] public float levelPassPercentage = 75f;
}
