using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage")]
public class Stage : ScriptableObject
{
    public new string name = "New Stage";
    public bool isUnlocked;
    public int numPassedLevels;
    [SerializeField] Level[] levels = null;

    public Level[] Levels => levels;
}
