using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectButton : MonoBehaviour
{

    int stage = 1;
    public void InitializeButton(int val) => stage = val;

    public void OnButtonPressed()
    {
        GameManager.Instance.SetCurrentStage(stage);
        SceneTransitions.PlayTransition(InTransition.CIRCLE_WIPE_LEFT, OutTransition.CIRCLE_WIPE_LEFT, LevelsMenu.Open);
    }
}
