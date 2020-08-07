using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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

    public int CurrentStageIndex { get => currentStageIndex; set => currentStageIndex = value; }
    public int CurrentLevelIndex { get => currentLevelIndex; set => currentLevelIndex = value; }
    public string DroneNote { get => droneNote; set => droneNote = value; }
    public InstrumentType Instrument { get => instrument; set => instrument = value; }

    [Header("Prefabs")]
    [SerializeField] MenuManagerUpdated menuManager = null;
    [SerializeField] AudioManager audioManager = null;
    [SerializeField] SceneTransitions sceneTransition = null;
    [SerializeField] UIAnimator uiAnimator = null;
    [SerializeField] Stage[] stages = null;
    [SerializeField] Game game = null;

    [Header("Variables")]
    [Range(1, 5)] [SerializeField] int currentStageIndex = 1;
    [Range(1, 10)] [SerializeField] int currentLevelIndex = 1;
    [SerializeField] string droneNote = "C4";
    [SerializeField] InstrumentType instrument = InstrumentType.HARMONIUM;

    public const string StartScene = "MenuScene";
    public const string GameScene = "GameScene";

    GameState currentState;


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
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        currentState = GameState.Running;

        Instantiate(menuManager);
        Instantiate(audioManager);
        Instantiate(sceneTransition);
        Instantiate(uiAnimator);
        if (GetCurrentSceneName() == GameScene) Instantiate(game);
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region UTILITY
    public void UnlockNextStage()
    {
        if (!IsFinalLevel() || IsFinalStage()) return;

        Stage nextStage = GetStage(currentStageIndex + 1);
        if (nextStage != null && !nextStage.isUnlocked)
        {
            nextStage.isUnlocked = true;
            BinarySaveSystem.SaveStageData();
        }
    }
    public void UnlockNextLevel()
    {
        if (IsFinalLevel()) return;
        Level nextLevel = GetLevel(currentLevelIndex + 1);
        if (nextLevel != null && !nextLevel.isUnlocked)
        {
            nextLevel.isUnlocked = true;
            BinarySaveSystem.SaveLevelData();
        }        
    }
    public bool IsCurrentStageCompleted()
    {
        if (currentStageIndex > GetNumStages()) return false;
        Stage currentStage = GetStages()[currentStageIndex - 1];
        //if (currentStage != null) return currentStage.isCompleted;
        return false;
    }
    
    public void IncrementStage()
    {
        if (currentStageIndex < GetNumStages()) currentStageIndex++;
    }
    public Stage[] GetStages() => stages;
    public int GetNumStages() => stages != null ? stages.Length : 0;
    public Level[] GetLevelsInCurrentStage()
    {
        return currentStageIndex > 0 && currentStageIndex <= stages.Length ? stages[currentStageIndex - 1].Levels : null;
    }
    public void IncrementLevel()
    {
        Level[] levels = GetLevelsInCurrentStage();
        if (levels != null && currentLevelIndex < levels.Length) currentLevelIndex++;
    }
    public bool IsFinalStage() => GetNumStages() == currentStageIndex;
    public bool IsFinalLevel() => GetNumLevelsInCurrentStage() == currentLevelIndex;
    public Level GetCurrentLevel() => GetLevel(currentLevelIndex);
    public Level GetLevel(int levelIndex)
    {
        Level[] levels = GetLevelsInCurrentStage();
        if (levels == null) return null;
        return levelIndex > 0 && levelIndex <= levels.Length ? levels[levelIndex - 1] : null;
    }
    public Stage GetStage(int stageIndex)
    {
        Stage[] stages = GetStages();
        if (stages == null) return null;
        return stageIndex > 0 && stageIndex <= stages.Length ? stages[stageIndex - 1] : null;
    }
    public int GetNumLevelsInCurrentStage()
    {
        Level[] levels = GetLevelsInCurrentStage();
        return levels != null ? levels.Length : 0;
    }

    #endregion    

    #region SCENE LOADING
    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneTransitions.sceneLoadingComplete = true;
    }
    public static string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
    public static int GetCurrentSceneIndex() => SceneManager.GetActiveScene().buildIndex;
    public void LoadGameScene() => SceneManager.LoadScene(GameScene);
    public static void LoadStartScene(Action action = null) => LoadScene(StartScene, action);
    public static void LoadScene(string levelName, Action action = null)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            if (levelName == StartScene) MainMenu.Instance.Open();
            if (levelName == GameScene) Instantiate(Instance.game);
            SceneManager.LoadScene(levelName);
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
            MenuManagerUpdated.CloseAllMenus();
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
