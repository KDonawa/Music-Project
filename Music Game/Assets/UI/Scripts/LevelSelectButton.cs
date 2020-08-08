using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText = null;
    [SerializeField] GameObject starsContainer = null;

    int levelIndex = 1;
    public void InitializeButton(int index, int numStars)
    {
        levelIndex = index;
        levelText.text = index.ToString();

        if (starsContainer == null) return;

        Image[] stars = starsContainer.GetComponentsInChildren<Image>();
        foreach (var star in stars) star.gameObject.SetActive(false);
        for (int i = 0; i < stars.Length && i < numStars; i++) stars[i].gameObject.SetActive(true);
    }   

    public void ButtonPressed(System.Action<Button> buttonPressedAction)
    {
        buttonPressedAction?.Invoke(GetComponent<Button>());
        UIAnimator.ButtonPressEffect3(GetComponent<Button>(), AudioManager.buttonSelect1);   
        GameManager.Instance.CurrentLevelIndex = levelIndex;
        SceneTransitions.PlayTransition(InTransition.CLOSE_VERTICAL, OutTransition.CIRCLE_SHRINK, GameManager.Instance.LoadGameScene); 
    }
}
