using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectKiraCard : MonoBehaviour
{
    [Header("Glowアニメーター")]
    public Animator GlowAnim;

    [Header("もうひとつの方のカード")]
    public SelectKiraCard Another;

    #region クリック時の処理
    public virtual void OnClick()
    {
        Another.Off();
        GlowAnim.SetInteger("IsLit", 1);
    }
    #endregion

    #region 非表示
    public void Off()
    {
        SetUp();
        this.gameObject.SetActive(false);
    }
    #endregion

    #region 初期化
    public void SetUp()
    {
        this.gameObject.SetActive(true);
        GlowAnim.SetInteger("IsLit", 0);
    }
    #endregion
}
