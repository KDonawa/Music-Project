using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectMenu : MenuGeneric<StageSelectMenu>
{
    [SerializeField] Button stageSelectButtonsPrefab = null;
    [SerializeField] GameObject buttonsContainer = null;

    private void Start()
    {
        InitializeButtons();
    }

    void InitializeButtons()
    {
        Stage[] stages = GameManager.Instance.GetStages();
        for (int i = 1; i <= GameManager.Instance.NumStages; i++)
        {
            Button b = Instantiate(stageSelectButtonsPrefab, buttonsContainer.transform);
            b.interactable = stages[i-1].isUnlocked;
            b.GetComponentInChildren<TextMeshProUGUI>().text = stages[i-1].name.ToString();

            StageSelectButton ssb = b.GetComponent<StageSelectButton>();
            ssb.InitializeButton(i);
            b.onClick.AddListener(delegate { ssb.OnButtonPressed(); });
        }
    }

    public void OnMainMenuPressed()
    {
        MainMenu.Open();
    }

    public void OnStageSelected(int val)
    {
        GameManager.Instance.SetCurrentStage(val);
        LevelsMenu.Open();
    }

}
