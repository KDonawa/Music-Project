using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const int START_SCENE_INDEX = 1;
    public static GameManager Instance { get; private set; }

    [SerializeField] Stage[] stages = null;
    public int NumStages => stages.Length;

    public int currentStageIndex = 1;
    public int currentLevelIndex = 1; // put range attr 1 - w/e

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #region UTILITY
    public void SetCurrentLevelIndex(int level) => currentLevelIndex = level;
    public void SetCurrentStageIndex(int stage) => currentStageIndex = stage;
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
    public Level GetFinalLevel()
    {
        Level[] levels = GetCurrentStageLevels();
        return levels[levels.Length - 1];
    }
    public bool IsFinalLevel()
    {
        //if (GetCurrentStageLevels() == null) return false;
        return GetCurrentStageLevels().Length == currentLevelIndex;
    }
    public int GetNumLevelsInStage()
    {
        Level[] levels = GetCurrentStageLevels();
        if (levels != null) return levels.Length;
        else return 0;
    }
    public void InitializeGame()
    {
        if (currentStageIndex > 0 && currentStageIndex <= stages.Length)
        {
            Instantiate(stages[currentStageIndex - 1]);
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
        if(currentStageIndex > 0 && currentStageIndex <= stages.Length)
        {
            return stages[currentStageIndex - 1].Levels;
        }
        return null;
    }
    #endregion

    #region SCENE LOADING
    public void LoadGameScene()
    {
        if (Application.CanStreamedLevelBeLoaded("Game"))
        {
            MenuManager.Instance.ClearMenuHistory();
            SceneManager.LoadScene("Game");            
        }
    }
    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public static void LoadStartScene()
    {
        LoadScene(START_SCENE_INDEX);
    }
    public static void LoadScene(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
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
