using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class FieldUnitEffect : MonoBehaviour
{
    [Header("エフェクトオブジェクトの親")]
    public GameObject EffectParent;

    public void Start()
    {
        EffectParent.SetActive(false);
    }

    [Header("パワーアップ")]
    public PowerUpEffect powerUpEffect;
}

#region パワーアップ
[Serializable]

public class PowerUpEffect
{
    [Header("エフェクト親")]
    public GameObject EffectParent;

    [Header("背景")]
    public Image BackGround;

    [Header("アニメーション親")]
    public GameObject AnimationParent;

    [Header("パワーアップテキスト")]
    public Text PowerUpText;

    [Header("矢印")]
    public Image Arrow;

    [Header("矢印アウトライン")]
    public Image ArrowOutline;

    public IEnumerator PowerUpEffectCoroutine(int plusPower)
    {
        bool end = false;
        var sequence = DOTween.Sequence();

        EffectParent.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < EffectParent.transform.parent.childCount; i++)
        {
            EffectParent.transform.parent.GetChild(i).gameObject.SetActive(false);
        }

        EffectParent.SetActive(true);

        PowerUpText.text = $"＋{plusPower}";

        BackGround.gameObject.SetActive(true);

        #region アニメーション
        #region 下から動いてくる
        float time = 0.25f;

        Arrow.color = new Color(Arrow.color.r, Arrow.color.g, Arrow.color.b, 1);
        ArrowOutline.color = new Color(ArrowOutline.color.r, ArrowOutline.color.g, ArrowOutline.color.b, 1);
        PowerUpText.color = new Color(PowerUpText.color.r, PowerUpText.color.g, PowerUpText.color.b, 1);

        AnimationParent.transform.localPosition = new Vector3(0, -3, 0);

        sequence = DOTween.Sequence();

        sequence
            .Append(AnimationParent.transform.DOLocalMove(Vector3.zero, time).SetEase(Ease.OutBack))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;
        #endregion

        yield return new WaitForSeconds(0.2f);

        #region フェードアウト
        float time1 = 0.1f;

        sequence = DOTween.Sequence();

        sequence
            .Append(DOTween.To(() => Arrow.color, (x) => Arrow.color = x, new Color(Arrow.color.r, Arrow.color.g, Arrow.color.b, 0), time1))
            .Join(DOTween.To(() => ArrowOutline.color, (x) => ArrowOutline.color = x, new Color(ArrowOutline.color.r, ArrowOutline.color.g, ArrowOutline.color.b, 0), time1))
            .Join(DOTween.To(() => PowerUpText.color, (x) => PowerUpText.color = x, new Color(PowerUpText.color.r, PowerUpText.color.g, PowerUpText.color.b, 0), time1))
            .AppendCallback(() => { end = true; });

        sequence.Play();

        yield return new WaitWhile(() => !end);
        end = false;
        #endregion
        #endregion

        EffectParent.transform.parent.gameObject.SetActive(false);
    }
}
#endregion