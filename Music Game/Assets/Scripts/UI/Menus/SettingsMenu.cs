using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;
using TMPro;
using KD.MusicGame.Utility.SaveSystem;

namespace KD.MusicGame.UI
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        [Header("Volume Settings UI")]
        [SerializeField] Button volumeSettingsButton = null;
        [SerializeField] GameObject volumeSettingsTab = null;
        [SerializeField] Slider slider1 = null;
        [SerializeField] Slider slider2 = null;
        [SerializeField] Slider slider3 = null;
        [SerializeField] Slider slider4 = null;

        [Header("Gameplay Settings UI")]
        [SerializeField] Button gameplaySettingsButton = null;
        [SerializeField] GameObject gameplaySettingsTab = null;
        [SerializeField] Slider slider5 = null;
        [SerializeField] Slider slider6 = null;
        [SerializeField] TextMeshProUGUI numRoundsTextGUI = null;

        [Header("Refs")]
        [SerializeField] Button backButton = null;        
        [SerializeField] Color selectedColor = new Color();
        [SerializeField] Color unSelectedColor = new Color();

        // save data
        public static Slider Slider1 => Instance.slider1;
        public static Slider Slider2 => Instance.slider2;
        public static Slider Slider3 => Instance.slider3;
        public static Slider Slider4 => Instance.slider4;
        public static Slider NoteSpeedSlider => Instance.slider5;
        public static Slider NumRoundsSlider => Instance.slider6;

        protected override void Awake()
        {
            base.Awake();

            if (backButton != null) backButton.onClick.AddListener(() =>
            {
                BinarySaveSystem.SaveSettings();
                ReturnToPreviousMenu();
            });
            if (volumeSettingsButton != null) volumeSettingsButton.onClick.AddListener(() =>
            {
                OpenVolumeSettingsTab();
                UIAnimator.ButtonPressEffect1(volumeSettingsButton, AudioManager.buttonSelect2);
            });
            if (gameplaySettingsButton != null) gameplaySettingsButton.onClick.AddListener(() =>
            {
                gameplaySettingsTab.SetActive(true);
                volumeSettingsTab.SetActive(false);
                volumeSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = unSelectedColor;
                gameplaySettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
                UIAnimator.ButtonPressEffect1(volumeSettingsButton, AudioManager.buttonSelect2);
            });

            slider1.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.NONE, slider1.value));
            slider2.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.DRONE, slider2.value));
            slider3.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.INSTRUMENT, slider3.value));
            slider4.onValueChanged.AddListener((value) => AudioManager.SetVolume(SoundType.SFX, slider4.value));
            slider6.onValueChanged.AddListener((value) => numRoundsTextGUI.text = value.ToString());

                      
        }

        public static void LoadData()
        {
            SettingsSaveData data = BinarySaveSystem.LoadSettingsData();
            if (data != null)
            {
                Instance.slider1.value = data.value1;
                Instance.slider2.value = data.value2;
                Instance.slider3.value = data.value3;
                Instance.slider4.value = data.value4;
                Instance.slider5.value = data.noteSpeed;
                Instance.slider6.value = data.numRounds;
            }
        }
        public override void Open()
        {
            OpenVolumeSettingsTab();
            base.Open();
        }

        void OpenVolumeSettingsTab()
        {
            gameplaySettingsTab.SetActive(false);
            volumeSettingsTab.SetActive(true);
            volumeSettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;
            gameplaySettingsButton.GetComponentInChildren<TextMeshProUGUI>().color = unSelectedColor;
        }
        void ReturnToPreviousMenu()
        {
            if (GameManager.GetCurrentSceneName() == GameManager.StartScene)
            {
                UIAnimator.ButtonPressEffect3(backButton, AudioManager.buttonSelect2);
                SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, MainMenu.Instance.Open);
            }
            else if (GameManager.GetCurrentSceneName() == GameManager.GameScene)
            {
                AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
                PauseMenu.Instance.Open();
            }
        }        
    }
}

