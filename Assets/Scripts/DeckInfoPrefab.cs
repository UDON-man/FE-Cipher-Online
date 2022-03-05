using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class DeckInfoPrefab : MonoBehaviour
{
    [Header("キーカード画像")]
    public Image KeyCardImage;

    [Header("デフォルトのキーカード画像")]
    public Sprite DefaultKeyCardSprite;

    [Header("デッキ名")]
    public Text DeckName;

    [Header("赤マーク")]
    public Image RedIcon;

    [Header("青マーク")]
    public Image BlueIcon;

    [Header("緑マーク")]
    public Image GreenIcon;

    [Header("黄マーク")]
    public Image YellowIcon;

    [Header("紫マーク")]
    public Image PurpleIcon;

    [Header("白マーク")]
    public Image WhiteIcon;

    [Header("黒マーク")]
    public Image BlackIcon;

    [Header("茶マーク")]
    public Image BrownIcon;

    [Header("無色マーク")]
    public Image ColorlessIcon;

    [Header("スクロール")]
    public ScrollRect scrollRect;

    [Header("アウトライン")]
    public GameObject Outline;

    [Header("主人公無し画像")]
    public Image ReverseFace;

    public DeckData thisDeckData {get;set;}

    //public UnityAction<DeckData> OnEnterAction;

    public UnityAction<DeckData> OnClickAction;

    private void OnEnable()
    {
        OnExit();
    }

    public void OnEnter()
    {
        this.gameObject.transform.localScale = Opening.instance.DeckInfoPrefabExpandScale;
    }

    public void OnClick()
    {
        OnClickAction?.Invoke(thisDeckData);

        Outline.SetActive(true);

        for (int j = 0; j < this.transform.parent.childCount; j++)
        {
            if (this.transform.parent.GetChild(j) != this.transform)
            {
                if(this.transform.parent.GetChild(j).GetComponent<DeckInfoPrefab>() != null)
                {
                    this.transform.parent.GetChild(j).GetComponent<DeckInfoPrefab>().Outline.SetActive(false);
                }

                if (this.transform.parent.GetChild(j).GetComponent<CreateNewDeckButton>() != null)
                {
                    this.transform.parent.GetChild(j).GetComponent<CreateNewDeckButton>().Outline.SetActive(false);
                }
            }
        }
    }

    public void OnExit()
    {
        if(Opening.instance != null)
        this.gameObject.transform.localScale = Opening.instance.DeckInfoPrefabStartScale;
    }

    public void SetUpDeckInfoPrefab(DeckData deckData)
    {
        this.gameObject.SetActive(deckData != null);

        if (deckData != null)
        {
            //キーカード画像
            //KeyCardImage.gameObject.SetActive(deckData.KeyCard != null);

            if (deckData.MainCharacter != null)
            {
                KeyCardImage.sprite = deckData.MainCharacter.CardImage;
                ReverseFace.gameObject.SetActive(false);
            }

            else
            {
                KeyCardImage.sprite = DefaultKeyCardSprite;
                ReverseFace.gameObject.SetActive(true);
            }

            //デッキ名
            DeckName.text = deckData.DeckName;

            //色シンボル
            RedIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Red)) > 0);
            BlueIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Blue)) > 0);
            GreenIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Green)) > 0);
            YellowIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Yellow)) > 0);
            PurpleIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Purple)) > 0);
            WhiteIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.White)) > 0);
            BlackIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Black)) > 0);
            BrownIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Brown)) > 0);
            ColorlessIcon.gameObject.SetActive(deckData.DeckCards().Count((card) => card.cardColors.Contains(CardColor.Colorless)) > 0);

            thisDeckData = deckData;

            Outline.SetActive(false);
        }
    }

}
