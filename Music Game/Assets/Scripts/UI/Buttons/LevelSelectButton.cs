using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class LevelSelectButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelText = null;
        [SerializeField] GameObject starsContainer = null;
        [SerializeField] Color earnedStarColor = new Color();
        [SerializeField] Color unearnedStarColor = new Color();

        int levelIndex = 0;
        public void InitializeButton(int index, int numStars)
        {
            levelIndex = index;
            levelText.text = $"{index + 1}";

            if (starsContainer == null) return;

            numStars = Mathf.Clamp(numStars, 0, 3);
            Image[] stars = starsContainer.GetComponentsInChildren<Image>();
            int i = 0;
            for (; i < stars.Length && i < numStars; i++) stars[i].color = earnedStarColor;
            for (; i < stars.Length; i++) stars[i].color = unearnedStarColor;
        }

        public void ButtonPressed(System.Action<Button> buttonPressedAction)
        {
            buttonPressedAction?.Invoke(GetComponent<Button>());
            UIAnimator.ButtonPressEffect3(GetComponent<Button>(), AudioManager.buttonSelect1);
            GameManager.Instance.currentLevelIndex = levelIndex;
            SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.FADE_OUT, GameManager.LoadGameScene);
        }
    }
}

