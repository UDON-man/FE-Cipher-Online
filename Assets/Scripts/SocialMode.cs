using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMode : MonoBehaviour
{
    [Header("ソーシャルボタン")]
    public OpeningButton SocialButton;

    [Header("ソーシャル画面パネル")]
    public GameObject SocialPanel;

    bool first = false;
    public void offSocial()
    {

        if(!first)
        {
            SocialButton.OnExit();
            first = true;
        }
        

        SocialPanel.SetActive(false);
    }

    public void SetUpSocialMode()
    {
        if (SocialPanel.activeSelf)
        {
            return;
        }

        //SocialButton.OnSelect();

        SocialPanel.SetActive(true);

        Animator anim = SocialPanel.GetComponent<Animator>();

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
    }
}
