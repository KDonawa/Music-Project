using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class LevelButton : MonoBehaviour
    {
        const int maxNumSubLevels = 6;
        public Button Button { get; private set; }

        public bool[] selectedSubLevels = new bool[maxNumSubLevels];
        public int numNotesPlayedPerRound = 1;
        StageCreationScreen stageCreation;
        

        private void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnButtonPressed);
        }
        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnButtonPressed);
        }
        public void Init(StageCreationScreen stageCreationScreen)
        {
            stageCreation = stageCreationScreen;
        }
        public void UpdateSelectedSubLevels(int index, bool newValue)
        {
            selectedSubLevels[index] = newValue;
        }
        public void UpdateNumNotesPlayed(int value) => numNotesPlayedPerRound = value;
        public void ResetButton()
        {
            selectedSubLevels = new bool[maxNumSubLevels];
            numNotesPlayedPerRound = 1;
        }
        public bool HasActiveNote()
        {
            foreach (var item in selectedSubLevels)
            {
                if (item) return true;
            }
            return false;
        }

        void OnButtonPressed()
        {
            if (stageCreation.ActiveLevelButton == this) return;

            UIAnimator.ButtonPressEffect1(Button, AudioManager.buttonSelect2);

            SelectButton();
        }
        public void SelectButton()
        {
            stageCreation.ActiveLevelButton = this;

            stageCreation.NumNotesPlayedSlider.value = numNotesPlayedPerRound;

            for (int i = 0; i < stageCreation.LevelButtons.Length; i++)
            {
                if (stageCreation.LevelButtons[i] != this) UIAnimator.SetButtonTextColor(stageCreation.LevelButtons[i].Button, stageCreation.unselectedColor);
                else UIAnimator.SetButtonTextColor(Button, stageCreation.selectedColor);
            }

            for (int i = 0; i < stageCreation.SubLevelButtons2.Length && i < selectedSubLevels.Length; i++)
            {
                stageCreation.SubLevelButtons2[i].HighlightButton(selectedSubLevels[i]);
                stageCreation.SubLevelButtons2[i].isSelected = selectedSubLevels[i];
            }
        }
    }
}