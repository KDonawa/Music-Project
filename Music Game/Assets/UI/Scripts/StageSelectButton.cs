using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButton : MonoBehaviour
{

    int stage = 1;

    public void InitializeButton(int val)
    {
        stage = val;
    }

    public void ButtonPressed(System.Action<Button> buttonPressedAction)
    {
        UIAnimator.ButtonPressEffect(GetComponent<Button>(), AudioManager.buttonChime);
        buttonPressedAction?.Invoke(GetComponent<Button>());
        GameManager.Instance.SetCurrentStage(stage);
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, LevelSelectMenu.Open);
    }
}
