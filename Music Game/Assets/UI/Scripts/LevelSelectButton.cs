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
        UIAnimator.ButtonPressEffect(GetComponent<Button>(), AudioManager.buttonChime);
        buttonPressedAction?.Invoke(GetComponent<Button>());

        GameManager.Instance.SetCurrentLevel(level);
        // probably play an animation first
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.CIRCLE_SHRINK, GameManager.Instance.LoadGameScene); 
    }
}
