using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsSaveData
{
    public float value1;
    public float value2;
    public float value3;
    public float value4;

    public SettingsSaveData()
    {
        value1 = SettingsMenu.Slider1.value;
        value2 = SettingsMenu.Slider2.value;
        value3 = SettingsMenu.Slider3.value;
        value4 = SettingsMenu.Slider4.value;
    }

}
