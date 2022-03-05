using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Linq;

public class DeckListPanel : MonoBehaviour
{
    public Animator anim;

    public CardPrefab_CreateDeck cardPrefab_CreateDeckPrefab;

    public DetailCard detailCard;

    public ScrollRect MainCharacterScroll;
    public ScrollRect DeckScroll;

    public Text DeckNameText;
    public Text DeckCountText;

    public GameObject DeckListPanelObject;

    public SwitchLanguage switchLanguage;
    public IEnumerator SetUpDeckListPanel(DeckData deckData)
    {
        if (switchLanguage != null)
        {
            switchLanguage.gameObject.SetActive(false);
        }

        detailCard.OffDetailCard();

        for(int i =0;i< MainCharacterScroll.content.childCount;i++)
        {
            Destroy(MainCharacterScroll.content.GetChild(i).gameObject);
        }

        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            Destroy(DeckScroll.content.GetChild(i).gameObject);
        }

        yield return new WaitWhile(() => DeckScroll.content.childCount > 0);

        #region デッキ名を表示
        DeckNameText.text = deckData.DeckName;
        #endregion

        #region 主人公・他のデッキのカードを取得
        List<CEntity_Base> DeckCards = new List<CEntity_Base>();
        List<CEntity_Base> MainCharacters = new List<CEntity_Base>();

        foreach (CEntity_Base cEntity_Base in deckData.DeckCards())
        {
            DeckCards.Add(cEntity_Base);
        }

        if(deckData.MainCharacter != null)
        {
            if(deckData.DeckCards().Contains(deckData.MainCharacter))
            {
                DeckCards.Remove(deckData.MainCharacter);
                MainCharacters.Add(deckData.MainCharacter);
            }
        }
        #endregion

        #region 主人公のカードを生成
        foreach (CEntity_Base cEntity_Base in MainCharacters)
        {
            CardPrefab_CreateDeck _cardPrefab_CreateDeck = null;

            _cardPrefab_CreateDeck = Instantiate(cardPrefab_CreateDeckPrefab, MainCharacterScroll.content);

            _cardPrefab_CreateDeck.OnEnterAction = () => { OnDetailCard(cEntity_Base, Vector3.zero); detailCard.transform.localPosition = new Vector3(638, 0, 0); };

            _cardPrefab_CreateDeck.OnExitAction = () => { OffDetailCard(); };

            SetUpDeckCard(_cardPrefab_CreateDeck, cEntity_Base, true);

            _cardPrefab_CreateDeck.HideDeckCardTab();
        }
        #endregion

        #region 他のデッキのカードを生成
        foreach (CEntity_Base cEntity_Base in DeckCards)
        {
            int index = DeckCards.IndexOf(cEntity_Base);

            CardPrefab_CreateDeck _cardPrefab_CreateDeck = null;

            _cardPrefab_CreateDeck = Instantiate(cardPrefab_CreateDeckPrefab, DeckScroll.content);

            _cardPrefab_CreateDeck.OnEnterAction = () => { OnDetailCard(cEntity_Base, Vector3.zero); detailCard.transform.localPosition = new Vector3(638, 0, 0); };

            _cardPrefab_CreateDeck.OnExitAction = () => { OffDetailCard(); };

            SetUpDeckCard(_cardPrefab_CreateDeck, cEntity_Base, false);

            _cardPrefab_CreateDeck.HideDeckCardTab();
        }
        #endregion

        SetDeckCountText(deckData);

        yield return new WaitWhile(() => DeckScroll.content.childCount < DeckCards.Count);

        this.gameObject.SetActive(true);

        yield return new WaitForSeconds(Time.deltaTime);

        DeckScroll.verticalNormalizedPosition = 1;

        StartCoroutine(OpenCoroutine(deckData));

        if (switchLanguage != null)
        {
            switchLanguage.gameObject.SetActive(true);
            switchLanguage.SetLanguage();
        }
    }

    IEnumerator OpenCoroutine(DeckData deckData)
    {
        yield return new WaitWhile(() => MainCharacterScroll.content.childCount + DeckScroll.content.childCount < deckData.DeckCards().Count);
        yield return new WaitForSeconds(Time.deltaTime);

        Open();

        yield return new WaitWhile(() => DeckListPanelObject.transform.localScale.y < 1.05f);

        //yield return new WaitForSeconds(Time.deltaTime);

        DeckScroll.verticalNormalizedPosition = 1;

        for(int i=0;i<MainCharacterScroll.content.childCount;i++)
        {
            MainCharacterScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().SetUpCardPrefab_CreateDeck(MainCharacterScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().cEntity_Base);
        }

        for (int i = 0; i < DeckScroll.content.childCount; i++)
        {
            DeckScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().SetUpCardPrefab_CreateDeck(DeckScroll.content.GetChild(i).GetComponent<CardPrefab_CreateDeck>().cEntity_Base);
        }

    }

    public void Open()
    {
        On();
        anim.SetInteger("Open", 1);
        anim.SetInteger("Close", 0);
    }

    public void Close()
    {
        OffDetailCard();
        anim.SetInteger("Open", 0);
        anim.SetInteger("Close", 1);
    }

    public void On()
    {
        this.gameObject.SetActive(true);
    }

    public void Off()
    {
        this.gameObject.SetActive(false);
    }

    #region カード詳細表示
    public void OffDetailCard()
    {
        detailCard.GetComponent<DetailCard>().OffDetailCard();
    }

    public void OnDetailCard(CEntity_Base cEntity_Base, Vector3 position)
    {
        detailCard.GetComponent<DetailCard>().SetUpDetailCard(cEntity_Base, position);
    }
    #endregion

    public void SetUpDeckCard(CardPrefab_CreateDeck _cardPrefab_CreateDeck, CEntity_Base cEntity_Base, bool isMainCharacter)
    {
        foreach (ScrollRect _scroll in _cardPrefab_CreateDeck.scroll)
        {
            if (!isMainCharacter)
            {
                _scroll.content = DeckScroll.content;

                _scroll.viewport = DeckScroll.viewport;
            }
        }

        _cardPrefab_CreateDeck.SetUpCardPrefab_CreateDeck(cEntity_Base);

        _cardPrefab_CreateDeck.Parent.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    #region デッキ枚数テキストを表示
    public void SetDeckCountText(DeckData deckData)
    {
        int MainCharacterCount = 0;

        if (deckData.MainCharacter != null)
        {
            if (deckData.DeckCards().Contains(deckData.MainCharacter))
            {
                MainCharacterCount = 1;
            }
        }

        DeckCountText.text = $"{deckData.DeckCards().Count}({deckData.DeckCards().Count - MainCharacterCount}+{MainCharacterCount})";

        if (deckData.DeckCards().Count >= 50 && MainCharacterCount == 1)
        {
            DeckCountText.color = new Color32(69, 255, 69, 255);
        }

        else
        {
            DeckCountText.color = new Color32(255, 64, 64, 255);
        }
    }
    #endregion
}
