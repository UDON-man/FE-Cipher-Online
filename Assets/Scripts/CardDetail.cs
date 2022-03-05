using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardDetail : MonoBehaviour
{
    [Header("カード詳細表示HandCard")]
    public HandCard DetailHandCard;

    public Text SwitchLanguageButtonText;

    public void SetLanguage()
    {
        if (ContinuousController.instance.language == Language.ENG)
        {
            SwitchLanguageButtonText.text = "To JPN";
        }

        else
        {
            SwitchLanguageButtonText.text = "To ENG";
        }

        DetailHandCard.SetLanguage();
    }

    public void OnClickSwitchLanguageButton()
    {
        if (ContinuousController.instance.language == Language.ENG)
        {
            ContinuousController.instance.language = Language.JPN;
        }

        else
        {
            ContinuousController.instance.language = Language.ENG;
        }

        SetLanguage();

        ContinuousController.instance.SaveLanguage();
    }

    public void OpenCardDetail(CardSource cardSource, bool CanLookOpponentCard)
    {
        this.gameObject.SetActive(true);

        DetailHandCard.SetUpHandCard(cardSource);

        if (CanLookOpponentCard)
        {
            DetailHandCard.SetUpHandCardImage();
        }

        DetailHandCard.PlayCostText.transform.parent.gameObject.SetActive(false);
        DetailHandCard.CCCostText.transform.parent.gameObject.SetActive(false);

        #region アニメーション
        DetailHandCard.transform.localPosition = new Vector3(400, 0, 0);
        DetailHandCard.transform.localScale = new Vector3(5.6f, 5.6f, 5.6f);

        float animationTime = 0.12f;

        var sequence = DOTween.Sequence();

        sequence
            .Append(DetailHandCard.transform.DOLocalMove(new Vector3(550, 0, 0), animationTime))
            .Join(DetailHandCard.transform.DOScale(new Vector3(7, 7, 7), animationTime));

        sequence.Play();

        #endregion

        SetLanguage();
    }

    public void CloseCardDetail()
    {
        this.gameObject.SetActive(false);
    }
}
