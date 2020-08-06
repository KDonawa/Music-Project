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

    public int NumStages => stages.Length;
    public int CurrentStageIndex { get => currentStageIndex; set => currentStageIndex = value; }
    public int CurrentLevelIndex { get => currentLevelIndex; set => currentLevelIndex = value; }
    public string DroneNote { get => droneNote; set => droneNote = value; }

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
    [SerializeField] string droneNote = "Test";

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
    public Stage[] GetStages() => stages;
    Level[] GetCurrentStageLevels()
    {
        if (currentStageIndex > 0 && currentStageIndex <= stages.Length)
        {
            return stages[currentStageIndex - 1].Levels;
        }
        return null;
    }
    public void IncrementLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        if (currentLevelIndex < levels.Length) ++currentLevelIndex;
    }
    public Level GetCurrentLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        return currentLevelIndex - 1 < levels.Length ? levels[currentLevelIndex - 1] : levels[0];
    }
    public bool IsFinalLevel()
    {
        //if (GetCurrentStageLevels() == null) return false;
        return GetCurrentStageLevels().Length == currentLevelIndex;
    }
    public int GetNumLevelsInCurrentStage()
    {
        Level[] levels = GetCurrentStageLevels();
        if (levels != null) return levels.Length;
        else return 0;
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
