using UnityEngine;

public class Stage : MonoBehaviour
{
    public new string name = "New Stage";
    public bool isUnlocked = false;
    [SerializeField] Level[] levels = null;

    public Level[] Levels => levels;
}
