namespace KD.MusicGame.Utility.SaveSystem
{
    [System.Serializable]
    public class SettingsSaveData
    {
        public float value1;
        public float value2;
        public float value3;
        public float value4;
        public float noteSpeed;

        public SettingsSaveData()
        {
            value1 = UI.SettingsMenu.Slider1.value;
            value2 = UI.SettingsMenu.Slider2.value;
            value3 = UI.SettingsMenu.Slider3.value;
            value4 = UI.SettingsMenu.Slider4.value;
            noteSpeed = UI.SettingsMenu.NoteSpeedSlider.value;
        }
    }
}

