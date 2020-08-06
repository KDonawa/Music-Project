using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public int level = 1;
    public void InitializeButton(int val) => level = val;    

    public void ButtonPressed(System.Action<Button> buttonPressedAction)
    {
        buttonPressedAction?.Invoke(GetComponent<Button>());
        UIAnimator.ButtonPressEffect3(GetComponent<Button>(), AudioManager.buttonChime);   
        GameManager.Instance.CurrentLevelIndex = level;
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.CIRCLE_SHRINK, GameManager.Instance.LoadGameScene); 
    }
}
