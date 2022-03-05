using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectKiraCardParent : MonoBehaviour
{
    [Header("アニメーター")]
    public Animator anim;

    [Header("マッチング方式決定カードボタン")]
    public List<SelectKiraCard> kiraCards = new List<SelectKiraCard>();

    public void Off()
    {
        this.gameObject.SetActive(false);
    }

    public void SetUpSelectBattleMode()
    {
        if (this.gameObject.activeSelf)
        {
            return;
        }

        this.gameObject.SetActive(true);

        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);


        foreach (SelectKiraCard kiraCard_SelectBattleMode in kiraCards)
        {
            kiraCard_SelectBattleMode.SetUp();
        }
    }
}
