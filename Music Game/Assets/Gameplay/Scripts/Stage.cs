using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage")]
public class Stage : ScriptableObject
{
    public new string name = "New Stage";
    public bool isUnlocked;
    [SerializeField] Level[] levels = null;

    public Level[] Levels => levels;
}
