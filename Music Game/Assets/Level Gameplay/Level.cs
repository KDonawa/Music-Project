﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    public Sound droneNote = null;
    public Sound[] notes = null;
    public int pointsPerCorrectGuess = 10;
    //public int maxScore;
    //[Range(50f, 100f)] public float levelPassPercentage = 75f;
}
