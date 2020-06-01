using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public const int START_SCENE_INDEX = 1;

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
        }
        else
        {
            Debug.LogWarning("LevelLoader.LoadLevel(string levelName) Error: invalid scene name specified");
        }
    }
    public static void LoadScene(int sceneIndex)
    {
        if(sceneIndex >=0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
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


    
}
