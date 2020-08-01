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
        //GameUI = Instantiate(gameUI);
        //GameplayUtility = Instantiate(gameplayUtility);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region UTILITY
    public Stage[] GetStages() => stages;
    public void SetCurrentLevel(int level) => currentLevel = level;
    public void SetCurrentStage(int stage) => currentStage = stage;
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
    public Level GetFinalLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        return levels[levels.Length - 1];
    }
    public bool IsFinalLevel()
    {
        //if (GetCurrentStageLevels() == null) return false;
        return GetCurrentStageLevels().Length == currentLevel;
    }
    public int GetNumLevelsInStage()
    {
        Level[] levels = GetCurrentStageLevels();
        if (levels != null) return levels.Length;
        else return 0;
    }
    public void InitializeGame()
    {
        if (currentStage > 0 && currentStage <= stages.Length)
        {
            Instantiate(stages[currentStage - 1]);
        }
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    #endregion

    #region HELPERS
    Level[] GetCurrentStageLevels()
    {
        if(currentStage > 0 && currentStage <= stages.Length)
        {
            return stages[currentStage - 1].Levels;
        }
        return null;
    }
    #endregion

    #region SCENE LOADING
    public static string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
    public void LoadGameScene() => SceneManager.LoadScene("GameScene");
    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public static void LoadStartScene() => LoadScene("MenuScene");

    public static void LoadScene(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            if(MenuManager.Instance != null) MenuManager.Instance.ClearMenuHistory();
            SceneManager.LoadScene(levelName);
            // instantiate the stage prefab here
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
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogWarning("LevelLoader.LoadLevel(int buildIndex) Error: invalid build index specified");
        }
    }
    public static void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void LoadNextScene()
    {
        //int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1);
        LoadScene(nextSceneIndex);
    }

    public void LoadPreviousScene()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        LoadScene(previousSceneIndex);
    }
    #endregion


}
