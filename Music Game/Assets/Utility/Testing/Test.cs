using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    Button b;
    [SerializeField] Test_SO testSO = null;

    private void Awake()
    {
        b = GetComponent<Button>();
    }
    private void Start()
    {
        b.onClick.AddListener(DoSomething);
    }

    public void DoSomething()
    {
        //UIAnimator.ShrinkAndExpand(b.GetComponent<RectTransform>(), 0.8f, 2f);
        //UIAnimator.SetColor(GetComponent<Image>(), Color.white);
        //UIAnimator.RotateZ(b.GetComponent<RectTransform>(), -360f, 1f);
        //UIAnimator.MoveY(b.GetComponent<RectTransform>(), -1f, 5f);
        //UIAnimator.ShrinkToNothing(b.GetComponent<RectTransform>(), 5f);
        //UIAnimator.Scale(b.GetComponent<RectTransform>(), 2f, 2f);
        //UITransition.CrossFade(2f);
        //UIAnimator.FlashButtonColor(b, Color.white, 0.5f);
        testSO.DoSomething();
    }
}
