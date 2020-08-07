using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Test_SO : ScriptableObject
{
    [SerializeField] int value;

    public void DoSomething()
    {
        value++;
    }
}
