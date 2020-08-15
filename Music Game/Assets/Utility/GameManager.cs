﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using KD.MusicGame.Gameplay;
using KD.MusicGame.Utility.SaveSystem;
using KD.MusicGame.UI;


namespace KD.MusicGame.Utility
{
    public enum GameState
    {
        Running,
        Paused,
        Loading,
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action GamePausedEvent;
        public static event Action GameUnPausedEvent;

        #region PROPERTIES
        public static int CurrentStageIndex
        {
            get => Instance.currentStageIndex;
            set => Instance.currentStageIndex = value > 0 && value <= Stages.Length ? value : 1;
        }
        public static int CurrentLevelIndex
        {
            get => Instance.currentLevelIndex;
            set => Instance.currentLevelIndex = value > 0 && value <= CurrentLevels.Length ? value : 1;
        }
        public static string DroneNote { get => Instance.droneNote; set => Instance.droneNote = value; }
        public static InstrumentType Instrument { get => Instance.instrument; set => Instance.instrument = value; }
        public static Stage[] Stages => Instance.stages;
        public static int NumStages => Instance.stages.Length;
        public static Stage CurrentStage => Instance.stages[Instance.currentStageIndex - 1];
        public static Level CurrentLevel => CurrentLevels[Instance.currentLevelIndex - 1];
        public static Level[] CurrentLevels => CurrentStage.Levels;
        #endregion

        [Header("Prefabs")]
        [SerializeField] WelcomeScreen welcomeScreen = null;
        [SerializeField] MenuManager menuManager = null;
        [SerializeField] AudioManager audioManager = null;
        [SerializeField] SceneTransitions sceneTransition = null;
        [SerializeField] UIAnimator uiAnimator = null;
        [SerializeField] Stage[] stages = null;
        [SerializeField] Gameplay.Game game = null;

        [Header("Variables")]
        [Range(1, 4)] [SerializeField] int currentStageIndex = 1;
        [Range(1, 10)] [SerializeField] int currentLevelIndex = 1;
        [SerializeField] string droneNote = "C4";
        [SerializeField] InstrumentType instrument = InstrumentType.HARMONIUM;

        public const string StartScene = "StartScene";
        public const string GameScene = "GameScene";
        public const string WelcomeScene = "WelcomeScene";

        GameState currentState;

        public bool isNewGame = true;


        #region GAME STATE
        public static void ChangeGameState(GameState gameState)
        {
            if (gameState == Instance.currentState) return;

            switch (gameState)
            {
                case GameState.Running:
                    Time.timeScale = 1f;
                    if (Instance.currentState == GameState.Paused) GameUnPausedEvent?.Invoke();
                    Instance.currentState = GameState.Running;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    GamePausedEvent?.Invoke();
                    Instance.currentState = GameState.Paused;
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region SETUP
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                GameSaveData data = BinarySaveSystem.LoadGameData();
                if (data != null) isNewGame = data.isNewGame;
                else isNewGame = true;

                currentState = GameState.Running;

                Instantiate(menuManager);
                Instantiate(audioManager);
                Instantiate(sceneTransition);
                Instantiate(uiAnimator);

                if (GetCurrentSceneName() == WelcomeScene)
                {
                    WelcomeScreen newWelcomescreen = Instantiate(welcomeScreen);
                    newWelcomescreen.gameObject.SetActive(false);
                    SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT,
                        () => newWelcomescreen.gameObject.SetActive(true));

                }
                if (GetCurrentSceneName() == GameScene)// for testing
                {
                    LoadGameData();
                    Instantiate(game); 
                }

            }
        }
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Gameplay.Game.LevelPassedEvent += UnlockNextStage;
            Gameplay.Game.LevelPassedEvent += UnlockNextLevel;
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                //Debug.Log("test");
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Gameplay.Game.LevelPassedEvent -= UnlockNextStage;
                Gameplay.Game.LevelPassedEvent -= UnlockNextLevel;
            }
        }

        #endregion

        #region SAVE DATA
        public static void LoadGameData()
        {
            Instance.LoadStageAndLevelData();
        }
        void LoadStageAndLevelData()
        {
            StageSaveData data = BinarySaveSystem.LoadStageData();
            if (data == null) return;
            for (int i = 0; i < NumStages; i++)
            {
                if (Stages[i] != null)
                {
                    Stages[i].isUnlocked = data.unlockedStages[i];
                    Stages[i].numPassedLevels = data.numPassedLevels[i];
                    LoadLevelData(i + 1);
                }
            }
        }
        public static void LoadLevelData(int index)
        {
            LevelSaveData data = BinarySaveSystem.LoadLevelData(index);
            if (data == null) return;

            for (int i = 0; i < CurrentLevels.Length; i++)
            {
                if(CurrentLevels[i] != null)
                {
                    CurrentLevels[i].isUnlocked = data.unlockedLevels[i];
                    CurrentLevels[i].numStarsEarned = data.starsEarned[i];
                    CurrentLevels[i].hiScore = data.hiScores[i];
                }              
            }
        }
        public static void StartNewGame()
        {
            Instance.isNewGame = false;
            BinarySaveSystem.SaveGameData();
            ResetStageAndLevelData();
        }
        public static void ResetStageAndLevelData()
        {
            for (int i = 0; i < Instance.stages.Length; i++)
            {
                if (Instance.stages[i] != null)
                {
                    if(i == 0) Instance.stages[i].isUnlocked = true;
                    else Instance.stages[i].isUnlocked = false;
                    Instance.stages[i].numPassedLevels = 0;
                    Instance.ResetLevelData(Instance.stages[i], i + 1);
                }
            }
            BinarySaveSystem.SaveStageData();
        }
        void ResetLevelData(Stage stage, int stageIndex)
        {
            //Level[] levels = stage.Levels;
            for (int i = 0; i < stage.Levels.Length; i++)
            {
                if(stage.Levels[i] != null)
                {
                    if (i == 0) stage.Levels[i].isUnlocked = true;
                    else stage.Levels[i].isUnlocked = false;
                    stage.Levels[i].isPassed = false;
                    stage.Levels[i].numStarsEarned = 0;
                    stage.Levels[i].hiScore = 0;
                    BinarySaveSystem.SaveLevelData(stageIndex);
                }
            }
        }


        #endregion

        #region UTILITY
        public static void UnlockNextStage()
        {
            //Debug.Log("attempt unlock next stage");
            if (!IsFinalLevel() || IsFinalStage()) return;

            Stage nextStage = Stages[Instance.currentStageIndex];
            if (nextStage != null && !nextStage.isUnlocked)
            {
                nextStage.isUnlocked = true;
            }
        }
        public static void UnlockNextLevel()
        {
            //Debug.Log("attempt unlock next level");
            if (IsFinalLevel()) return;
            Level nextLevel = CurrentLevels[Instance.currentLevelIndex];
            if (nextLevel != null && !nextLevel.isUnlocked)
            {
                nextLevel.isUnlocked = true;
            }
        }
        public static Level[] GetLevelsInStage(int stageIndex)
        {
            return stageIndex > 0 && stageIndex <= NumStages ? Instance.stages[stageIndex - 1].Levels : null; ;
        }
        public static void IncrementStage()
        {
            if (Instance.currentStageIndex < NumStages) Instance.currentStageIndex++;
        }
        public static void IncrementLevel()
        {
            if (Instance.currentLevelIndex < CurrentLevels.Length) Instance.currentLevelIndex++;
        }
        public static bool IsFinalStage() => NumStages == Instance.currentStageIndex;
        public static bool IsFinalLevel() => CurrentStage.Levels.Length == Instance.currentLevelIndex;

        #endregion

        #region SCENE LOADING
        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == GameScene) Instantiate(game);
            //if (scene.name == StartScene) MainMenu.Instance.Open();
            SceneTransitions.sceneLoadingComplete = true;
        }
        public static string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
        public static int GetCurrentSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        public static void LoadGameScene() => SceneManager.LoadScene(GameScene);
        public static void LoadStartScene(Action action = null) => LoadScene(StartScene, action);
        public static void LoadScene(string levelName, Action action = null)
        {
            if (Application.CanStreamedLevelBeLoaded(levelName))
            {
                SceneManager.LoadScene(levelName);
                if (levelName == StartScene) MainMenu.Instance.Open();
                action?.Invoke();
            }
            else
            {
                Debug.LogWarning("LevelLoader.LoadLevel(string levelName) Error: invalid scene name specified");
            }
        }
        public static void LoadScene(int sceneIndex)
        {
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                //if (MenuManager.Instance != null) MenuManager.Instance.ClearMenuHistory();
                MenuManager.CloseAllMenus();
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogWarning("LevelLoader.LoadLevel(int buildIndex) Error: invalid build index specified");
            }
        }
        public static void ReloadScene() => LoadScene(SceneManager.GetActiveScene().buildIndex);
        public static void LoadNextScene()
        {
            //int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            //int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1);
            //LoadScene(nextSceneIndex);
        }
        public void LoadPreviousScene()
        {
            //int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
            //LoadScene(previousSceneIndex);
        }
        #endregion

        #region HELPERS
        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        #endregion
    }
}

