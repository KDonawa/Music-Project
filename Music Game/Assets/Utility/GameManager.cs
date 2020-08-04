using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public const int START_SCENE_INDEX = 1;
    public static GameManager Instance { get; private set; }

    [SerializeField] SceneTransitions sceneTransition = null;
    [SerializeField] UIAnimator uiAnimator = null;
    [SerializeField] GameUI gameUI = null;
    [SerializeField] LevelGameplayUtility gameplayUtility = null;
    [SerializeField] Stage[] stages = null;

    public GameUI GameUI => gameUI;
    public LevelGameplayUtility GameplayUtility => gameplayUtility;
    public int NumStages => stages.Length;

    public int currentStage = 1;
    public int currentLevel = 1; // put range attr 1 - w/e

    public const string StartScene = "MenuScene";
    public const string GameScene = "GameScene";

    #region SETUP
    private void Awake()
    {
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Instantiate(sceneTransition);
        Instantiate(uiAnimator);
    }
    private void Start()
    {        
        SceneManager.sceneLoaded += InitializeGame;
        if(GetCurrentSceneName() == "GameScene") InstantiateGame();
    }
    void InitializeGame(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != "GameScene") return;
        InstantiateGame();
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion

    #region STAGE UTILITY
    public Stage[] GetStages() => stages;
    public void SetCurrentStage(int stage) => currentStage = stage;
    Level[] GetCurrentStageLevels()
    {
        if (currentStage > 0 && currentStage <= stages.Length)
        {
            return stages[currentStage - 1].Levels;
        }
        return null;
    }
    #endregion

    #region LEVEL UTILITY
    public void SetCurrentLevel(int level) => currentLevel = level;

    public void IncrementLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        if (currentLevel < levels.Length) ++currentLevel;
    }
    public Level GetCurrentLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        return currentLevel - 1 < levels.Length ? levels[currentLevel - 1] : levels[0];
    }
    public bool IsFinalLevel()
    {
        //if (GetCurrentStageLevels() == null) return false;
        return GetCurrentStageLevels().Length == currentLevel;
    }
    public int GetNumLevelsInCurrentStage()
    {
        Level[] levels = GetCurrentStageLevels();
        if (levels != null) return levels.Length;
        else return 0;
    }

    #endregion    

    #region SCENE LOADING
    public static string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
    public static int GetCurrentSceneIndex() => SceneManager.GetActiveScene().buildIndex;
    public void LoadGameScene() => SceneManager.LoadScene(GameScene);
    public static void LoadStartScene() => LoadScene(StartScene);
    public static void LoadScene(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            if(MenuManager.Instance != null) MenuManager.Instance.ClearMenuHistory();
            SceneManager.LoadScene(levelName);
            //SceneManager.LoadSceneAsync(levelName);
            SceneTransitions.sceneLoadingComplete = true;
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
            if (MenuManager.Instance != null) MenuManager.Instance.ClearMenuHistory();
            SceneManager.LoadScene(sceneIndex);
            SceneTransitions.sceneLoadingComplete = true;
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
    public void InstantiateGame()
    {
        if (currentStage > 0 && currentStage <= stages.Length)
        {
            Instantiate(stages[currentStage - 1]);
        }
    }
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
