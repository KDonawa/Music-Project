using KD.MusicGame.Gameplay;
using KD.MusicGame.Utility;
using KD.MusicGame.Utility.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KD.MusicGame.UI
{
    public class HiScorePanel : MonoBehaviour
    {
        [SerializeField] GameObject hiScoreSlotsContainer = null;
        [SerializeField] HiScoreSlot hiScoreSlotPrefab = null;
        [SerializeField] Button closeButton = null;

        List<HiScoreSlot> _hiScoreSlots;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButtonPressed);
            _hiScoreSlots = new List<HiScoreSlot>();
        }
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnCloseButtonPressed);
        }

        public void Open()
        {
            //Gameplay.Level[] levels = GameManager.CurrentLevels;
            LevelData[] levels = GameManager.GetCurrentStage().levels;
            int i = 0;
            for (; i < levels.Length && i < _hiScoreSlots.Count; i++)
            {
                if(levels[i] != null)
                {
                    _hiScoreSlots[i].SetLevelNumberText(i + 1);
                    _hiScoreSlots[i].SetHiScoreText(levels[i].hiScore);
                    _hiScoreSlots[i].gameObject.SetActive(true);
                }                
            }
            for (; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    _hiScoreSlots.Add(Instantiate(hiScoreSlotPrefab, hiScoreSlotsContainer.transform));
                    _hiScoreSlots[i].SetLevelNumberText(i + 1);
                    _hiScoreSlots[i].SetHiScoreText(levels[i].hiScore);
                }                    
            }
            for (; i < _hiScoreSlots.Count; i++) _hiScoreSlots[i].gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        void OnCloseButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(closeButton, AudioManager.buttonSelect2);
            gameObject.SetActive(false);
        }
    }
}

