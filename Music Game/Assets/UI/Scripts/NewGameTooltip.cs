using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class NewGameTooltip : MonoBehaviour
    {
        [SerializeField] Button yesButton = null;
        [SerializeField] Button noButton = null;

        private void Awake()
        {
            yesButton.onClick.AddListener(YesClicked);
            noButton.onClick.AddListener(NoClicked);
        }
        private void OnDestroy()
        {
            yesButton.onClick.RemoveListener(YesClicked);
            noButton.onClick.RemoveListener(NoClicked);
        }

        void YesClicked()
        {
            UIAnimator.ButtonPressEffect3(yesButton, AudioManager.buttonSelect1);
            GameManager.StartNewGame();
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.CIRCLE_SHRINK, DroneSelectMenu.Instance.Open);
        }
        void NoClicked()
        {
            UIAnimator.ButtonPressEffect3(noButton, AudioManager.buttonSelect2);
            gameObject.SetActive(false);
        }
    }
}

