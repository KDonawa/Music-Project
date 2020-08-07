using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float value1 = 1f;
    public float value2 = 1f;
    public float value3 = 1f;

    public SettingsData()
    {
        value1 = SettingsMenu.Instance.Value1;
        value2 = SettingsMenu.Instance.Value2;
        value3 = SettingsMenu.Instance.Value3;
    }

}
