using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningButton : MonoBehaviour
{
    [Header("ボタンアニメーター")]
    public Animator ButtonAnimator;
    public void OnSelect()
    {
        if (ButtonAnimator != null)
        {
            ButtonAnimator.SetInteger("IsOpen", 1);
        }
            
    }

    public void OnExit()
    {
        if (ButtonAnimator != null)
        {
            ButtonAnimator.SetInteger("IsOpen", -1);
        }
    }
}
