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
        int numStages = GameManager.Instance.NumStages;
        for (int i = 1; i <= numStages; i++)
        {
            Button b = Instantiate(stageSelectButtonsPrefab, buttonsContainer.transform);
            b.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();

            StageSelectButton ssb = b.GetComponent<StageSelectButton>();
            ssb.InitializeButton(i);
            b.onClick.AddListener(delegate { ssb.OnButtonPressed(); });
        }
    }

    public void OnMainMenuPressed()
    {
        MainMenu.Open();
    }

    public void OnStageSelected(int index)
    {
        GameManager.Instance.SetCurrentStageIndex(index);
        LevelsMenu.Open();
    }

}
