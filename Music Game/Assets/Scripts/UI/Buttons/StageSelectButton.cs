using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;
using KD.MusicGame.Gameplay;

namespace KD.MusicGame.UI
{
    public class StageSelectButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI stageName = null;
        [SerializeField] TextMeshProUGUI levelsPassed = null;
        int stageIndex = 0;

        public void Init(int position, StageData stage)
        {
            stageIndex = position;
            stageName.text = $"{position + 1}. {stage.name}";
            if (levelsPassed == null) return;
            levelsPassed.text = $"{stage.numPassedLevels}/{stage.levels.Length}";
        }

        public void ButtonPressed(System.Action<Button> buttonPressedAction)
        {
            UIAnimator.ButtonPressEffect3(GetComponent<Button>(), AudioManager.buttonSelect1);
            buttonPressedAction?.Invoke(GetComponent<Button>());
            GameManager.Instance.currentStageIndex = stageIndex;
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_VERTICAL, LevelSelectMenu.Instance.Open);
        }
    }
}


