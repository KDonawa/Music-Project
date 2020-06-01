using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    public int level = 1;
    public void InitializeButton(int val) => level = val;    

    public void OnLoadLevelButtonPressed()
    {
        GameManager.Instance.SetCurrentLevelIndex(level);
        GameManager.Instance.LoadGameScene();
        
    }
}
