using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelsMenu : MenuGeneric<LevelsMenu>
{    
    [SerializeField] Button levelSelectButtonsPrefab = null;
    [SerializeField] GameObject buttonsContainer = null;

    private void Start()
    {
        InitializeButtons();
    }

    void InitializeButtons()
    {
        int numLevels = GameManager.Instance.GetNumLevelsInStage();
        for (int i = 1; i <= numLevels; i++)
        {
            Button b = Instantiate(levelSelectButtonsPrefab, buttonsContainer.transform);
            b.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            LevelSelectButton lsb = b.GetComponent<LevelSelectButton>();
            lsb.InitializeButton(i);
            b.onClick.AddListener(delegate { lsb.OnLoadLevelButtonPressed(); });
        }
    }
    public void OnMainMenuPressed()
    {
        MainMenu.Open();
    }
    public void OnStageMenuPressed()
    {
        StageSelectMenu.Open();
    }
    
}
