using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceButton : MonoBehaviour
{

    public void Initialize(string name)
    {
        NoteName = name;
    }

    public string NoteName { get; private set; }

    
}
