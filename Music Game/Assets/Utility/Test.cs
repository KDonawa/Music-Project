using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] Button b;

    public void DoSomething()
    {
        //UIAnimator.ShrinkAndExpand(b.GetComponent<RectTransform>(), 0.8f, 2f);
        //UIAnimator.ChangeImageColor(b.GetComponent<Image>(), Color.green);
        //UIAnimator.RotateZ(b.GetComponent<RectTransform>(), -360f, 1f);
        //UIAnimator.MoveY(b.GetComponent<RectTransform>(), -1f, 5f);
        //UIAnimator.ShrinkToNothing(b.GetComponent<RectTransform>(), 5f);
        //UIAnimator.Scale(b.GetComponent<RectTransform>(), 2f, 2f);
        //UITransition.CrossFade(2f);
    }
}
