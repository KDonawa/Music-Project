using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;
using KD.MusicGame.Gameplay;
using KD.MusicGame.Utility.SaveSystem;
using System.Collections.Generic;

namespace KD.MusicGame.UI
{
    public class StageCreationScreen : MonoBehaviour
    {
        readonly string[] notes = { ".pa", "._dha", ".dha", "._ni", ".ni", "sa", "_re", "re", "_ga", "ga", "ma", "Ma", "pa", "_dha",
            "dha","_ni","ni","'sa","'_re","'re","'_ga","'ga","'ma","'Ma","'pa"};
        [SerializeField] string[] subLevels = { "A", "B", "C", "D", "E", "F" };        
        [Range(0,10)][SerializeField] int maxNumLevels = 10;
        [SerializeField] GameObject tooltip = null;

        public Color unselectedColor = new Color();
        public Color selectedColor = new Color();

        [Header("Stage Options")]
        [SerializeField] GameObject stageOptions = null;
        [SerializeField] Slider numLevelsSlider = null;
        [SerializeField] Slider numSubLevelsSlider = null;
        [SerializeField] TextMeshProUGUI numLevelsTextGUI = null;
        [SerializeField] TextMeshProUGUI numSubLevelsTextGUI = null;
        [SerializeField] Button nextButton1 = null;
        [SerializeField] Button cancelButton = null;

        [Header("Sub-Level Options")]
        [SerializeField] GameObject subLevelOptions = null;
        [SerializeField] GameObject subLevelButtonsContainer = null;
        [SerializeField] SubLevelButton subLevelButtonPrefab = null;
        [SerializeField] GameObject noteButtonsContainer = null;
        [SerializeField] NoteButton noteButtonPrefab = null;
        [SerializeField] Button nextButton2 = null;
        [SerializeField] Button previousButton1 = null;        
        public SubLevelButton[] SubLevelButtons { get; private set; }
        public NoteButton[] NoteButtons { get; private set; }
        public SubLevelButton ActiveSubLevelButton { get; set; }
        int numSubLevels = 0;

        [Header("Level Options")]
        [SerializeField] GameObject levelOptions = null;
        [SerializeField] GameObject levelButtonsContainer = null;
        [SerializeField] LevelButton levelButtonPrefab = null;
        [SerializeField] Slider numNotesPlayedSlider = null;
        [SerializeField] TextMeshProUGUI numNotesTextGUI = null;
        [SerializeField] GameObject subLevelButtonsContainer2 = null;
        [SerializeField] SubLevelButton2 subLevelButton2Prefab = null;
        [SerializeField] Button confirmButton = null;
        [SerializeField] Button previousButton2 = null;
        public LevelButton[] LevelButtons { get; private set; }
        public SubLevelButton2[] SubLevelButtons2 { get; private set; }
        public LevelButton ActiveLevelButton { get; set; }
        public Slider NumNotesPlayedSlider => numNotesPlayedSlider;
        int numLevels = 0;

        private void Awake()
        {
            // init sliders
            numLevelsSlider.onValueChanged.AddListener((value) =>
            {
                numLevelsTextGUI.text = value.ToString();
                AudioManager.PlaySound(AudioManager.sliderChanged, SoundType.SFX);
            });
            numSubLevelsSlider.onValueChanged.AddListener((value) =>
            {
                numSubLevelsTextGUI.text = value.ToString();
                AudioManager.PlaySound(AudioManager.sliderChanged, SoundType.SFX);
            });
            numNotesPlayedSlider.onValueChanged.AddListener((value) => 
            {
                numNotesTextGUI.text = value.ToString();
                if(ActiveLevelButton != null) ActiveLevelButton.UpdateNumNotesPlayed((int)value);
                AudioManager.PlaySound(AudioManager.sliderChanged, SoundType.SFX);
            });

            // init navigation buttons events
            nextButton1.onClick.AddListener(NextButton1Pressed);            
            cancelButton.onClick.AddListener(CancelButtonPressed);
            nextButton2.onClick.AddListener(NextButton2Pressed);
            previousButton1.onClick.AddListener(PreviousButton1Pressed);
            confirmButton.onClick.AddListener(ConfirmButtonPressed);
            previousButton2.onClick.AddListener(PreviousButton2Pressed);


            // init buttons
            SubLevelButtons = new SubLevelButton[subLevels.Length];
            for (int i = 0; i < SubLevelButtons.Length; i++)
            {
                SubLevelButtons[i] = Instantiate(subLevelButtonPrefab, subLevelButtonsContainer.transform);
                SubLevelButtons[i].Init(this, subLevels[i], notes.Length);
            }

            NoteButtons = new NoteButton[notes.Length];
            for (int i = 0; i < NoteButtons.Length; i++)
            {
                NoteButtons[i] = Instantiate(noteButtonPrefab, noteButtonsContainer.transform);
                NoteButtons[i].Init(i, notes[i], this);
            }

            LevelButtons = new LevelButton[maxNumLevels];
            for (int i = 0; i < LevelButtons.Length; i++)
            {
                LevelButtons[i] = Instantiate(levelButtonPrefab, levelButtonsContainer.transform);
                LevelButtons[i].Init(this, i + 1, subLevels.Length);
            }

            SubLevelButtons2 = new SubLevelButton2[subLevels.Length];
            for (int i = 0; i < SubLevelButtons2.Length; i++)
            {
                SubLevelButtons2[i] = Instantiate(subLevelButton2Prefab, subLevelButtonsContainer2.transform);
                SubLevelButtons2[i].Init(this, i, subLevels[i]);
            }
        }

        private void OnEnable()
        {
            if (tooltip != null) tooltip.SetActive(false);
            for (int i = 0; i < SubLevelButtons.Length; i++) SubLevelButtons[i].ResetButton(); // Init SubLevel Options
            for (int i = 0; i < LevelButtons.Length; i++) LevelButtons[i].ResetButton(); // Init Level Options
            OpenStageOptionsScreen();            
        }

        void OpenStageOptionsScreen()
        {
            if (stageOptions != null) stageOptions.gameObject.SetActive(true);
            if (subLevelOptions != null) subLevelOptions.gameObject.SetActive(false);
            if (levelOptions != null) levelOptions.gameObject.SetActive(false);
        }
        void OpenSubLevelOptionsScreen()
        {
            if (stageOptions != null) stageOptions.gameObject.SetActive(false);
            if (subLevelOptions != null) subLevelOptions.gameObject.SetActive(true);
            if (levelOptions != null) levelOptions.gameObject.SetActive(false);

            ActiveSubLevelButton = null;
            numSubLevels = (int)numSubLevelsSlider.value;

            if (SubLevelButtons.Length > 0)
            {
                SubLevelButtons[0].SelectButton();

                for (int i = 0; i < SubLevelButtons.Length; i++)
                {
                    if (i < numSubLevels) SubLevelButtons[i].gameObject.SetActive(true);
                    else SubLevelButtons[i].gameObject.SetActive(false);
                }
            }            
        }
        void OpenLevelOptionsScreen()
        {
            if (stageOptions != null) stageOptions.gameObject.SetActive(false);
            if (subLevelOptions != null) subLevelOptions.gameObject.SetActive(false);
            if (levelOptions != null) levelOptions.gameObject.SetActive(true);

            ActiveLevelButton = null;
            numLevels = (int)numLevelsSlider.value;
            numSubLevels = (int)numSubLevelsSlider.value;

            // display level options
            if (LevelButtons.Length > 0)
            {
                LevelButtons[0].SelectButton();

                for (int i = 0; i < LevelButtons.Length; i++)
                {
                    if (i < numLevels) LevelButtons[i].gameObject.SetActive(true);
                    else LevelButtons[i].gameObject.SetActive(false);
                }
            }

            // display saved sub-level selections
            for (int i = 0; i < SubLevelButtons2.Length; i++)
            {
                if (i < numSubLevels) SubLevelButtons2[i].gameObject.SetActive(true);
                else SubLevelButtons2[i].gameObject.SetActive(false);
            }
        }

        void NextButton1Pressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            OpenSubLevelOptionsScreen();
        }
        void CancelButtonPressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            gameObject.SetActive(false);
        }
        void NextButton2Pressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            
            //make sure we have valid selection in each sublevel
            for (int i = 0; i < numSubLevels && i < SubLevelButtons.Length; i++)
            {
                if (!SubLevelButtons[i].HasActiveNote())
                {
                    if (tooltip != null)
                    {
                        tooltip.GetComponentInChildren<TextMeshProUGUI>().text = "Must select at least 1 note in each Sublevel to continue";
                        tooltip.gameObject.SetActive(true);
                    }
                    return;
                }
            }

            OpenLevelOptionsScreen();
        }
        void PreviousButton1Pressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            OpenStageOptionsScreen();
        }
        void ConfirmButtonPressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);

            //make sure we have valid selection in each level
            for (int i = 0; i < numLevels && i < LevelButtons.Length; i++)
            {
                if (!LevelButtons[i].HasActiveNote())
                {
                    if (tooltip != null)
                    {
                        tooltip.GetComponentInChildren<TextMeshProUGUI>().text = "Must select at least 1 Sublevel in each Level to continue";
                        tooltip.gameObject.SetActive(true);
                    }
                    return;
                }
            }
            CreateStage();
        }
        void CreateStage()
        {
            StageData stageData = new StageData
            {
                isUnlocked = true,
                levels = new LevelData[numLevels]
            };            

            // init levels
            for (int j = 0; j < numLevels && j < LevelButtons.Length; j++)
            {
                LevelData levelData = new LevelData
                {
                    numNotesToGuess = LevelButtons[j].numNotesPlayedPerRound,
                    subLevels = ConstructSubLevelData(LevelButtons[j].selectedSubLevels)
                };
                if (j == 0) levelData.isUnlocked = true;
                stageData.levels[j] = levelData;
            }
            GameManager.customStagesList.Add(stageData);
            BinarySaveSystem.SaveCustomGameData();

            StageSelectMenu.Instance.InitializeMenu();
            gameObject.SetActive(false);
        }
        string[][] ConstructSubLevelData(bool[] selectedSubLevels)
        {
            List<string[]> subLevels = new List<string[]>();
            for (int i = 0; i < numSubLevels && i < selectedSubLevels.Length; i++)
            {
                if (selectedSubLevels[i])
                {
                    List<string> subLevelNotes = new List<string>();
                    for (int j = 0; j < SubLevelButtons[i].selectedNotes.Length; j++)
                    {
                        if (SubLevelButtons[i].selectedNotes[j]) subLevelNotes.Add(NoteButtons[j].note);
                    }
                    subLevels.Add(subLevelNotes.ToArray());
                }
            }           

            return subLevels.ToArray();
        }
        void PreviousButton2Pressed()
        {
            AudioManager.PlaySound(AudioManager.buttonSelect2, SoundType.SFX);
            OpenSubLevelOptionsScreen();
        }
    }
}