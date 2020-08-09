﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stageName = null;
    [SerializeField] TextMeshProUGUI levelsPassed = null;
    int stage = 1;

    public void Init(int val, string text, int numPassedLevels)
    {
        stage = val;
        stageName.text = val + ". " + text;
        if (levelsPassed == null) return;
        levelsPassed.text = numPassedLevels.ToString() + "/" + GameManager.CurrentLevels.Length.ToString();
    }

    public void ButtonPressed(System.Action<Button> buttonPressedAction)
    {
        UIAnimator.ButtonPressEffect3(GetComponent<Button>(), AudioManager.buttonSelect1);
        buttonPressedAction?.Invoke(GetComponent<Button>());
        GameManager.CurrentStageIndex = stage;
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.OPEN_HORIZONTAL, LevelSelectMenu.Instance.Open);
    }
}
