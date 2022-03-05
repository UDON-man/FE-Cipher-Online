using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
public class CharacterInfo : MonoBehaviour
{
    [Header("背景")]
    public Image BackGround;

    [Header("カード画像")]
    public Image CardImage;

    [Header("カード画像背景")]
    public Image CardImageBackGround;

    [Header("パワーテキスト")]
    public Text PowerText;

    [Header("スキルテキスト")]
    public List<Text> SkillTexts = new List<Text>();

    public ScrollRect scrollRect;

    UnityAction OnEnterAction;
    UnityAction OnExitAction;

    public CardSource cardSource { get; set; }

    public void OnEnter()
    {
        OnEnterAction?.Invoke();
    }

    public void OnExit()
    {
        OnExitAction?.Invoke();
    }

    public void SetUpCharacterInfo(CardSource cardSource,UnityAction _OnEnterAction,UnityAction _OnExitAction)
    {
        this.cardSource = cardSource;

        this.gameObject.SetActive(true);

        CardImage.color = new Color(1, 1, 1, 1);
        CardImage.sprite = cardSource.cEntity_Base.CardImage;
        CardImageBackGround.color = DataBase.CardColor_ColorDarkDictionary[cardSource.cEntity_Base.cardColors[0]];
        BackGround.color = DataBase.CardColor_ColorLightDictionary[cardSource.cEntity_Base.cardColors[0]];

        PowerText.transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        PowerText.text = cardSource.cEntity_Base.Power.ToString();

        for(int i=0;i<SkillTexts.Count;i++)
        {
            if (i < cardSource.cEntity_Base.SkillNames.Count)
            {
                SkillTexts[i].gameObject.SetActive(true);
                SkillTexts[i].text = $"・{cardSource.cEntity_Base.SkillNames[i]}";
            }

            else
            {
                SkillTexts[i].gameObject.SetActive(false);
            }
        }

        OnEnterAction = _OnEnterAction;
        OnExitAction = _OnExitAction;
    }

    public void CloseSoulInfo()
    {
        this.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        GManager.instance.cardDetail.OpenCardDetail(cardSource, true);
    }
}
