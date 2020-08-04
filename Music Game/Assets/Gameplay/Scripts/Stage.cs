using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Stage")]
public class Stage : ScriptableObject
{
    public new string name = "New Stage";
    public bool isUnlocked = false;
    [SerializeField] Level[] levels = null;

    public Level[] Levels => levels;
}
