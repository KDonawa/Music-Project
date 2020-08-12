using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KD.MusicGame.Utility;
namespace KD.MusicGame.UI
{
    public class LevelCompleteMenu : Menu<LevelCompleteMenu>
    {
        [Header("Buttons")]
        //[SerializeField] Button mainMenuButton = null;
        [SerializeField] Button homeButton = null;
        [SerializeField] Button restartButton = null;
        [SerializeField] Button nextLevelButton = null;

        [Header("UI")]
        [SerializeField] TextMeshProUGUI winText = null;
        [SerializeField] TextMeshProUGUI loseText = null;
        [SerializeField] TextMeshProUGUI scoreHeaderText = null;
        [SerializeField] TextMeshProUGUI scoreText = null;
        [SerializeField] GameObject starsContainer = null;
        [SerializeField] Image badgeIcon = null;

        Image[] stars;


        #region SETUP
        protected override void Awake()
        {
            base.Awake();
            if (starsContainer != null) stars = starsContainer.GetComponentsInChildren<Image>();

            if (homeButton == null) Debug.LogError("No reference to Home button");
            else homeButton.onClick.AddListener(OnHomeButtonPressed);

            if (restartButton == null) Debug.LogError("No reference to Restart button");
            else restartButton.onClick.AddListener(OnRestartPressed);

            if (nextLevelButton == null) Debug.LogError("No reference to Next Lvl button");
            else nextLevelButton.onClick.AddListener(OnNextLevelPressed);

            //mainMenuButton.onClick.AddListener(MainMenuPressed);

        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (homeButton != null) homeButton.onClick.RemoveListener(OnHomeButtonPressed);
            if (restartButton != null) restartButton.onClick.RemoveListener(OnRestartPressed);
            if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(OnNextLevelPressed);
            //mainMenuButton.onClick.RemoveListener(MainMenuPressed);

        }

        #endregion

        public static void DisplayMenu(int finalScore, bool isLevelPassed, int numStars)
        {
            // hide text
            Instance.winText.gameObject.SetActive(false);
            Instance.loseText.gameObject.SetActive(false);
            Instance.scoreHeaderText.gameObject.SetActive(false);
            Instance.scoreText.gameObject.SetActive(false);

            // deactivate stars
            for (int i = 0; i < Instance.stars.Length; i++)
            {
                Instance.stars[i].gameObject.SetActive(false);
            }
            Instance.badgeIcon.gameObject.SetActive(false);

            // hide buttons
            Instance.homeButton.gameObject.SetActive(false);
            Instance.restartButton.gameObject.SetActive(false);
            Instance.nextLevelButton.gameObject.SetActive(false);

            Instance.Open();

            Instance.StartCoroutine(Instance.DisplayMenuRoutine(finalScore, isLevelPassed, numStars));
        }

        public IEnumerator DisplayMenuRoutine(int finalScore, bool isLevelPassed, int numStars)
        {
            //yield return new WaitForSeconds(1f);

            // set header text
            if (isLevelPassed) winText.gameObject.SetActive(true);
            else loseText.gameObject.SetActive(true);
            if (isLevelPassed) AudioManager.PlaySound(AudioManager.success, SoundType.SFX);
            yield return new WaitForSeconds(1f);

            // set final score text routine     
            scoreText.text = "0%";
            scoreHeaderText.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            int increaingScore = 0;
            int counter = 0;
            int updateFrequency = 1;
            AudioManager.PlaySound(AudioManager.finalScoreUpdate, SoundType.SFX);
            while (increaingScore <= finalScore)
            {
                if (counter % updateFrequency == 0)
                {
                    scoreText.text = increaingScore.ToString() + "%";
                    increaingScore++;
                }
                counter++;
                yield return null;
            }
            AudioManager.StopSound(AudioManager.finalScoreUpdate, SoundType.SFX);
            yield return new WaitForSeconds(0.5f);



            // display stars
            numStars = Mathf.Clamp(numStars, 0, stars.Length);
            for (int i = 0; i < numStars; i++)
            {
                stars[i].gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.starDisplay, SoundType.SFX);
                yield return new WaitForSeconds(0.5f);
            }
            if (numStars == 3)
            {
                badgeIcon.gameObject.SetActive(true);
                AudioManager.PlaySound(AudioManager.badgeDisplay, SoundType.SFX);
            }
            yield return new WaitForSeconds(1f);

            // open stage complete menu
            if (isLevelPassed && GameManager.IsFinalLevel())
            {
                //GameManager.Instance.UnlockNextStage();
                yield return new WaitForSeconds(1f);
                StageCompleteMenu.Instance.Open();
            }
            // or navigation display buttons
            else
            {
                homeButton.gameObject.SetActive(true);
                homeButton.interactable = false;
                AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.SFX);
                yield return new WaitForSeconds(0.2f);

                restartButton.gameObject.SetActive(true);
                restartButton.interactable = false;
                AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.SFX);
                yield return new WaitForSeconds(0.2f);

                if (isLevelPassed && !GameManager.IsFinalLevel())
                {
                    //GameManager.Instance.UnlockNextLevel();
                    nextLevelButton.gameObject.SetActive(true);
                    AudioManager.PlaySound(AudioManager.buttonLoad1, SoundType.SFX);
                }

                homeButton.interactable = true;
                restartButton.interactable = true;
            }
        }


        #region BUTTON EVENTS
        //void MainMenuPressed()
        //{
        //    UIAnimator.ButtonPressEffect3(mainMenuButton, AudioManager.buttonSelect2);
        //    SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.CIRCLE_SHRINK, MainMenu.Instance.Open);
        //}

        void OnHomeButtonPressed()
        {
            UIAnimator.ButtonPressEffect3(homeButton, AudioManager.buttonSelect2);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_UP, OutTransition.OPEN_VERTICAL,
                () => GameManager.LoadStartScene(LevelSelectMenu.Instance.Open));
        }
        void OnRestartPressed()
        {
            UIAnimator.ButtonPressEffect3(restartButton, AudioManager.buttonSelect2);

            Gameplay.Game.Stop();

            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, Gameplay.Game.Play);
        }
        void OnNextLevelPressed()
        {
            UIAnimator.ButtonPressEffect3(nextLevelButton, AudioManager.buttonSelect1);
            SceneTransitions.PlayTransition(InTransition.CIRCLE_EXPAND, OutTransition.FADE_OUT, Gameplay.Game.PlayNextLevel);
        }
        #endregion

    }
}

