using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KD.MusicGame.Utility;

namespace KD.MusicGame.UI
{
    public class StageCompleteMenu : Menu<StageCompleteMenu>
    {
        [Header("Buttons")]
        [SerializeField] Button homeButton = null;
        [SerializeField] Button nextButton = null;

        protected override void Awake()
        {
            base.Awake();

            homeButton.onClick.AddListener(HomeButtonPressed);
            nextButton.onClick.AddListener(NextButtonPressed);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            homeButton.onClick.RemoveListener(HomeButtonPressed);
            nextButton.onClick.RemoveListener(NextButtonPressed);
        }
        public override void Open()
        {
            Instance.homeButton.gameObject.SetActive(false);
            Instance.nextButton.gameObject.SetActive(false);

            base.Open();

            StartCoroutine(DisplayMenuRoutine());
        }


        IEnumerator DisplayMenuRoutine()
        {
            AudioManager.PlaySound(AudioManager.stageComplete, SoundType.SFX);

            yield return new WaitForSeconds(2f);

            Instance.homeButton.gameObject.SetActive(true);
            AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.SFX);
            yield return new WaitForSeconds(0.2f);

            if (!GameManager.IsFinalStage())
            {
                Instance.nextButton.gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.SFX);
            }
        }


        #region BUTTON EVENTS
        void HomeButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(homeButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.OPEN_HORIZONTAL,
                () => GameManager.LoadStartScene(StageSelectMenu.Instance.Open));
        }
        void NextButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(nextButton, AudioManager.buttonSelect1);
            GameManager.IncrementStage();
            SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_VERTICAL,
                () => GameManager.LoadStartScene(LevelSelectMenu.Instance.Open));
        }
        #endregion
    }
}

