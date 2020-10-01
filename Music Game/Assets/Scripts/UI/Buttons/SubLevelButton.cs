using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class SubLevelButton : MonoBehaviour
    {
        public Button Button { get; private set; }

        public bool[] selectedNotes;
        int maxNumNotes = 1;

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
        public void Init(StageCreationScreen stageCreationScreen, int maxNum)
        {
            stageCreation = stageCreationScreen;
            maxNumNotes = maxNum;
            selectedNotes = new bool[maxNumNotes];
        }
        public void UpdateSelectedNotes(int index, bool newValue)
        {
            selectedNotes[index] = newValue;
        }
        public void ResetButton()
        {
            selectedNotes = new bool[maxNumNotes];
        }
        public bool HasActiveNote()
        {
            foreach (var item in selectedNotes)
            {
                if (item) return true;
            }
            return false;
        }

        void OnButtonPressed()
        {
            if (stageCreation.ActiveSubLevelButton == this) return;

            UIAnimator.ButtonPressEffect1(Button, AudioManager.buttonSelect2);

            SelectButton();
        }
        public void SelectButton()
        {
            stageCreation.ActiveSubLevelButton = this;

            for (int i = 0; i < stageCreation.SubLevelButtons.Length; i++)
            {
                if (stageCreation.SubLevelButtons[i] != this) UIAnimator.SetButtonTextColor(stageCreation.SubLevelButtons[i].Button, stageCreation.unselectedColor);
                else UIAnimator.SetButtonTextColor(Button, stageCreation.selectedColor);
            }

            for (int i = 0; i < stageCreation.NoteButtons.Length && i < selectedNotes.Length; i++)
            {
                stageCreation.NoteButtons[i].isSelected = selectedNotes[i];
                stageCreation.NoteButtons[i].HighlightButton(selectedNotes[i]);                
            }
        }
    }
}