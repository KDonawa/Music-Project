using System.Collections.Generic;
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
            set => Instance.currentStageIndex = value > 0 && value <= Instance.stagesList.Count ? value : 1;
        }
        public static int CurrentLevelIndex
        {
            get => Instance.currentLevelIndex;
            set => Instance.currentLevelIndex = value > 0 && value <= GetCurrentStage().levels.Length ? value : 1;
        }
        public static string DroneNote { get => Instance.droneNote; set => Instance.droneNote = value; }
        public static InstrumentType Instrument { get => Instance.instrument; set => Instance.instrument = value; }

        #endregion

        #region SETUP
        [Header("Prefabs")]
        [SerializeField] WelcomeScreen welcomeScreen = null;
        [SerializeField] MenuManager menuManager = null;
        [SerializeField] AudioManager audioManager = null;
        [SerializeField] SceneTransitions sceneTransition = null;
        [SerializeField] UIAnimator uiAnimator = null;
        [SerializeField] Game game = null;

        [Header("Stages")]
        [SerializeField] Stage[] stages = null;
        //[SerializeField] Stage[] customStages = null;

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

        public List<StageData> stagesList = new List<StageData>();
        public List<StageData> customStagesList = new List<StageData>();      

        
        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                GameStateSaveData data = BinarySaveSystem.LoadGameStateData();
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
            Game.LevelPassedEvent += UnlockNextStage;
            Game.LevelPassedEvent += UnlockNextLevel;
        }
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Game.LevelPassedEvent -= UnlockNextStage;
                Game.LevelPassedEvent -= UnlockNextLevel;
            }
        }

        #endregion

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

        #region SAVE DATA
        public static void StartNewGame()
        {
            Instance.isNewGame = false;
            BinarySaveSystem.SaveGameStateData();
            Instance.InitializeGameData();
        }
        void InitializeGameData()
        {
            //init stages
            stagesList.Clear();
            for (int i = 0; i < stages.Length; i++)
            {
                StageData stageData = new StageData
                {
                    name = stages[i].name,
                    isUnlocked = stages[i].isUnlocked,
                    numPassedLevels = stages[i].numPassedLevels,
                    levels = new LevelData[stages[i].Levels.Length]
                };
                stagesList.Add(stageData);

                // init levels
                for (int j = 0; j < stages[i].Levels.Length; j++)
                {
                    LevelData levelData = new LevelData
                    {
                        isUnlocked = stages[i].Levels[j].isUnlocked,
                        isPassed = stages[i].Levels[j].isPassed,
                        hiScore = stages[i].Levels[j].hiScore,
                        numStarsEarned = stages[i].Levels[j].numStarsEarned,
                        numNotesToGuess = stages[i].Levels[j].numNotesToGuess,
                        subLevels = new string[stages[i].Levels[j].subLevels.Length][]
                    };
                    stageData.levels[j] = levelData;

                    // init sublevels
                    for (int k = 0; k < stages[i].Levels[j].subLevels.Length; k++)
                    {
                        levelData.subLevels[k] = stages[i].Levels[j].subLevels[k].notes;
                    }
                }
            }
            
            BinarySaveSystem.SaveGameData();
        }        
        public static void LoadGameData()
        {
            GameSaveData data = BinarySaveSystem.LoadGameData();
            if (data == null) return;

            Instance.stagesList.Clear();
            for (int i = 0; i < data.stageNames.Length; i++)
            {
                StageData stageData = new StageData
                {
                    name = data.stageNames[i],
                    isUnlocked = data.unlockedStages[i],
                    numPassedLevels = data.numPassedLevels[i],
                    levels = new LevelData[data.stageNames.Length]
                };
                Instance.stagesList.Add(stageData);

                for (int j = 0; j < stageData.levels.Length; j++)
                {
                    LevelData levelData = new LevelData
                    {
                        isUnlocked = data.unlockedLevels[j],
                        isPassed = data.passedLevels[j],
                        hiScore = data.hiScores[j],
                        numStarsEarned = data.starsEarned[j],
                        numNotesToGuess = data.numNotesToGuess[j],
                        subLevels = data.subLevels[j]
                    };
                    stageData.levels[j] = levelData;
                }

            }
        }

        #endregion

        #region UTILITY
        public static StageData GetCurrentStage()
        {
            return Instance.stagesList[CurrentStageIndex - 1];
        }
        public static LevelData GetCurrentLevelData()
        {
            return GetCurrentStage().levels[CurrentLevelIndex - 1];
        }
        public static void UnlockNextStage()
        {
            if (!IsFinalLevel() || IsFinalStage()) return;

            StageData nextStage = Instance.stagesList[CurrentStageIndex];
            if (nextStage != null && !nextStage.isUnlocked)
            {
                nextStage.isUnlocked = true;
            }
        }
        public static void UnlockNextLevel()
        {
            if (IsFinalLevel()) return;

            LevelData nextLevel = GetCurrentStage().levels[CurrentLevelIndex];
            if (nextLevel != null && !nextLevel.isUnlocked)
            {
                nextLevel.isUnlocked = true;
            }
        }
        public static void IncrementStage()
        {
            if (Instance.currentStageIndex < Instance.stagesList.Count) Instance.currentStageIndex++;
        }
        public static void IncrementLevel()
        {
            if (!IsFinalLevel()) Instance.currentLevelIndex++;
        }
        public static bool IsFinalStage() => Instance.stagesList.Count == Instance.currentStageIndex;
        public static bool IsFinalLevel() => GetCurrentStage().levels.Length == Instance.currentLevelIndex;

        #endregion

        #region SCENE LOADING
        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == GameScene) Instantiate(game);

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
                MenuManager.CloseAllMenus();
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogWarning("LevelLoader.LoadLevel(int buildIndex) Error: invalid build index specified");
            }
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

