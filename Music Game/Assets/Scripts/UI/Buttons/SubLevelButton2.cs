using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class SubLevelButton2 : MonoBehaviour
    {
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
        public void Init(StageCreationScreen stageCreationScreen, int index, string textToDisplay)
        {
            this.index = index;
            GetComponentInChildren<TextMeshProUGUI>().text = textToDisplay;
            stageCreation = stageCreationScreen;
        }

        void OnButtonSelected()
        {
            isSelected = !isSelected;

            if (isSelected)
            {
                UIAnimator.ButtonPressEffect2(Button, AudioManager.buttonSelect2, stageCreation.selectedColor);
            }
            else
            {
                UIAnimator.ButtonPressEffect2(Button, AudioManager.buttonSelect2, stageCreation.unselectedColor);
            }
            stageCreation.ActiveLevelButton.UpdateSelectedSubLevels(index, isSelected);
        }
        public void HighlightButton(bool isActive)
        {
            if (isActive) UIAnimator.SetButtonTextColor(Button, stageCreation.selectedColor);
            else UIAnimator.SetButtonTextColor(Button, stageCreation.unselectedColor);
        }
    }
}