using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;
using KD.MusicGame.Gameplay;

namespace KD.MusicGame.UI
{
    public class CustomStageSelectButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI stageName = null;
        [SerializeField] TextMeshProUGUI levelsPassed = null;

        [HideInInspector] public StageData stage;
        int stageIndex = 0;        

        public void Init(int position, StageData stage)
        {
            this.stage = stage;
            stageIndex = position;
            stageName.text = $"{position + 1}. {stage.name}";
            if (levelsPassed == null) return;
            levelsPassed.text = $"{stage.numPassedLevels}/{stage.levels.Length}";
        }

        public void ButtonPressed()
        {
            UIAnimator.ButtonPressEffect1(GetComponent<Button>(), AudioManager.buttonSelect2);
            GameManager.Instance.currentStageIndex = stageIndex;
        }
    }
}