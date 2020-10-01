using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class NoteButton : MonoBehaviour
    {
        public string note;
        public int index = 0;
        public bool isSelected = false;
        public Button Button { get; private set; }
        StageCreationScreen stageCreation;

        private void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnButtonSelected);
        }
        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnButtonSelected);
        }
        public void Init(int index, StageCreationScreen stageCreationScreen)
        {
            this.index = index;
            stageCreation = stageCreationScreen;            
        }

        void OnButtonSelected()
        {
            isSelected = !isSelected;

            if(isSelected)
            {
                UIAnimator.ButtonPressEffect2(Button, AudioManager.buttonSelect2, stageCreation.selectedColor);
            }
            else
            {
                UIAnimator.ButtonPressEffect2(Button, AudioManager.buttonSelect2, stageCreation.unselectedColor);
            }
            stageCreation.ActiveSubLevelButton.UpdateSelectedNotes(index, isSelected);
        }
        public void HighlightButton(bool isActive)
        {
            if(isActive) UIAnimator.SetButtonTextColor(Button, stageCreation.selectedColor);
            else UIAnimator.SetButtonTextColor(Button, stageCreation.unselectedColor);
        }
    }
}