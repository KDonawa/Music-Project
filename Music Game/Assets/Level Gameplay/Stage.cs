using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] Level[] levels = null;

    public Level[] Levels => levels;
}
