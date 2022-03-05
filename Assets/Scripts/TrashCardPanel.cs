using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class TrashCardPanel : MonoBehaviour
{
    [Header("メッセージテキスト")]
    public Text MessageText;

    [Header("スクロールレクト")]
    public ScrollRect scrollRect;

    [Header("背景")]
    public GameObject BackGround;

    [Header("背景以外の親")]
    public GameObject Parent;

    #region 退避ゾーンのカードを確認
    public void OnClickCheckTrashButton(bool IsYou)
    {
        string Message = "";

        Player player = null;

        if (IsYou)
        {
            player = GManager.instance.You;
            Message = "Your Retreat Area Cards";
        }

        else
        {
            player = GManager.instance.Opponent;
            Message = "Opponent's Retreat Area Cards";
        }

        OpenSelectCardPanel(Message, player.TrashCards,true);
    }
    #endregion

    #region 無限ゾーンのカードを確認
    public void OnClickCheckInfinityButton(bool IsYou)
    {
        string Message = "";

        Player player = null;

        if (IsYou)
        {
            player = GManager.instance.You;
            Message = "Your Boundless Area Cards";
        }

        else
        {
            player = GManager.instance.Opponent;
            Message = "Opponent's Boundless Area Cards";
        }

        OpenSelectCardPanel(Message, player.InfinityCards,true);
    }
    #endregion

    #region オーブのカードを確認
    public void OnClickCheckOrbButton(bool IsYou)
    {
        string Message = "";

        Player player = null;

        if (IsYou)
        {
            player = GManager.instance.You;
            Message = "Your Orb Cards";
        }

        else
        {
            player = GManager.instance.Opponent;
            Message = "Opponent's Orb Cards";
        }

        OpenSelectCardPanel(Message, player.OrbCards,false);
    }
    #endregion

    #region 絆ゾーンのカードを確認
    public void OnClickCheckBondButton(bool IsYou)
    {
        string Message = "";

        Player player = null;

        if (IsYou)
        {
            player = GManager.instance.You;
            Message = "Your Bond Cards";
        }

        else
        {
            player = GManager.instance.Opponent;
            Message = "Opponent's Bond Cards";
        }

        OpenSelectCardPanel(Message, player.BondCards,true);
    }
    #endregion

    public void OpenSelectCardPanel(string Message, List<CardSource> RootCardSources,bool CanLookReverseCard)
    {
        #region 初期化・初期設定
        this.gameObject.SetActive(true);

        BackGround.SetActive(true);

        Parent.SetActive(false);

        MessageText.text = Message;
        #endregion

        StartCoroutine(OpenSelectCardPanelCoroutine(RootCardSources, CanLookReverseCard));
    }

    IEnumerator OpenSelectCardPanelCoroutine(List<CardSource> RootCardSources, bool CanLookReverseCard)
    {
        List<CardSource> root = new List<CardSource>();

        foreach(CardSource cardSource in RootCardSources)
        {
            root.Add(cardSource);
        }

        yield return new WaitForSeconds(Time.deltaTime);

        #region カード一覧を初期化
        while (scrollRect.content.childCount > 0)
        {
            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                if (scrollRect.content.GetChild(i) != null)
                {
                    if (scrollRect.content.GetChild(i).gameObject != null)
                    {
                        Destroy(scrollRect.content.GetChild(i).gameObject);
                        yield return null;
                    }
                }
            }
        }

        yield return new WaitWhile(() => scrollRect.content.childCount > 0);
        #endregion

        #region カード生成
        foreach (CardSource cardSource in root)
        {
            HandCard handCard = Instantiate(GManager.instance.handCardPrefab, scrollRect.content);

            handCard.GetComponent<Draggable_HandCard>().startScale = new Vector3(2.7f, 2.7f, 1);

            handCard.GetComponent<Draggable_HandCard>().DefaultY = -292;

            EventTrigger eventTrigger = handCard.CardImage.GetComponent<EventTrigger>();

            eventTrigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((x) => { PointerClick(cardSource); });

            #region クリック時の処理
            void PointerClick(CardSource cardSource1)
            {
                #region 右クリック
                if (Input.GetMouseButtonUp(1))
                {
                    if (cardSource1 != null)
                    {
                        //裏向きのカードを見ることができない
                        if (!CanLookReverseCard)
                        {
                            if (!cardSource1.IsReverse)
                            {
                                if (cardSource1.cEntity_Base == null)
                                {
                                    return;
                                }

                                GManager.instance.cardDetail.OpenCardDetail(cardSource1, true);
                            }
                        }

                        //裏向きのカードを見ることができる
                        else
                        {
                            if (cardSource1.cEntity_Base == null)
                            {
                                return;
                            }

                            GManager.instance.cardDetail.OpenCardDetail(cardSource1, true);
                        }
                    }
                }
                #endregion
            }
            #endregion

            eventTrigger.triggers.Add(entry);

            handCard.SetUpHandCard(cardSource);

            if (!cardSource.IsReverse)
            {
                handCard.SetUpHandCardImage();
            }

            else
            {
                handCard.SetUpReverseCard();
            }

            

            yield return null;
        }

        yield return new WaitWhile(() => scrollRect.content.childCount < root.Count);

        yield return new WaitForSeconds(Time.deltaTime * 2);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            scrollRect.content.GetChild(i).localScale = new Vector3(2.7f, 2.7f, 1);
        }
        #endregion

        Parent.SetActive(true);

        scrollRect.horizontalNormalizedPosition = 0;

        yield return new WaitForSeconds(Time.deltaTime * 1.5f);
    }


    public void CloseSelectCardPanel()
    {
        this.gameObject.SetActive(false);
    }
}
