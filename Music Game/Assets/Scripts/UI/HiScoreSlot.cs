using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KD.MusicGame.UI
{
    public class HiScoreSlot : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelNumberTextGUI = null;
        [SerializeField] TextMeshProUGUI hiScoreTextGUI = null;

        public void SetLevelNumberText(int level)
        {
            levelNumberTextGUI.text = level.ToString();
        }
        public void SetHiScoreText(int hiScore)
        {
            hiScoreTextGUI.text = hiScore.ToString() + "%";
        }
    }
}

