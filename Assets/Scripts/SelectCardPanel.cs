using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SelectCardPanel : MonoBehaviour
{
    [Header("メッセージテキスト")]
    public Text MessageText;

    [Header("スクロールレクト")]
    public ScrollRect scrollRect;

    [Header("選択しないボタン")]
    public GameObject NoSelectButton;

    [Header("選択決定ボタン")]
    public GameObject EndSelectButton;

    [Header("背景")]
    public GameObject BackGround;

    [Header("背景以外の親")]
    public GameObject Parent;

    [Header("カード選択に戻るボタン")]
    public GameObject ReturnToSelectCardButton;

    [Header("選択しないボタンテキスト")]
    public Text NotSelectButtonText;

    [Header("選択終了ボタンテキスト")]
    public Text EndSelectButtonText;

    public Text CountText;

    //選択リスト
    public List<CardSource> SelectedList { get; set; } = new List<CardSource>();
    public List<int> SelectedIndex { get; set; } = new List<int>();
    //List<CardSource> RootCardSources { get; set; } = new List<CardSource>();
    //対象となるカードの条件
    Func<CardSource, bool> CanTargetCondition { get; set; }
    //現在の選択リストの状態でそのユニットを選択することができるか
    Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    //選択終了することのできる条件(選択終了時点のリストを参照)
    Func<List<CardSource>, bool> CanEndSelectCondition { get; set; }
    //選択できる最大枚数
    public int MaxCount { get; set; }
    //最大枚数以下でも終了できるかどうか
    public bool CanEndNotMax { get; set; }
    //選択しないことを選べるか
    public Func<bool> CanNoSelect { get; set; }

    //仮選択リスト
    //public List<CardSource> PreSelectedList { get; set; } = new List<CardSource>();
    public List<HandCard> PreSelectedHandCardList { get; set; } = new List<HandCard>();
    //選択が終了したか
    public bool IsEndSelection { get; set; }
    //スクロールビューのカード一覧
    public List<HandCard> HandCards { get; set; } = new List<HandCard>();

    UnityAction OnClickNotSelectButtonAction;
    UnityAction OnClickEndSelectButtonAction;

    #region 選択が終わったことを通知
    public void SetIsEndSelection(bool _IsEndSelection)
    {
        IsEndSelection = _IsEndSelection;
    }
    #endregion

    //普通にカードを選択
    public IEnumerator OpenSelectCardPanel(string Message, List<CardSource> RootCardSources, Func<CardSource, bool> _CanTargetCondition, Func<List<CardSource>, CardSource, bool> _CanTargetCondition_ByPreSelecetedList, Func<List<CardSource>, bool> _CanEndSelectCondition, int _MaxCount, bool _CanEndNotMax, Func<bool> _CanNoSelect, bool CanLookReverseCard, List<SkillInfo> skillInfos)
    {
        yield return ContinuousController.instance.StartCoroutine(OpenSelectCardPanel(Message, "Not Select", "End Selection",null,null,RootCardSources,_CanTargetCondition,_CanTargetCondition_ByPreSelecetedList,_CanEndSelectCondition,_MaxCount,_CanEndNotMax,_CanNoSelect,CanLookReverseCard, skillInfos));
    }

    //カードを選択(ボタンテキストやクリック時の処理をカスタム)
    public IEnumerator OpenSelectCardPanel(string Message, string NotSelectButtonMessage, string EndSelectButtonMessage, UnityAction _OnClickNotSelectButtonAction, UnityAction _OnClickEndSelectButtonAction, List<CardSource> RootCardSources, Func<CardSource, bool> _CanTargetCondition, Func<List<CardSource>, CardSource, bool> _CanTargetCondition_ByPreSelecetedList, Func<List<CardSource>, bool> _CanEndSelectCondition, int _MaxCount, bool _CanEndNotMax, Func<bool> _CanNoSelect, bool CanLookReverseCard, List<SkillInfo> skillInfos)
    {
        #region 初期化・初期設定
        this.gameObject.SetActive(true);

        //this.RootCardSources = RootCardSources;

        OnClickNotSelectButtonAction = _OnClickNotSelectButtonAction;
        OnClickEndSelectButtonAction = _OnClickEndSelectButtonAction;

        NotSelectButtonText.text = NotSelectButtonMessage;
        EndSelectButtonText.text = EndSelectButtonMessage;

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            GManager.instance.turnStateMachine.OffFieldCardTarget(player);
            GManager.instance.turnStateMachine.OffHandCardTarget(player);
        }

        GManager.instance.turnStateMachine.IsSelecting = true;

        BackGround.SetActive(true);

        Parent.SetActive(false);

        SetIsEndSelection(false);

        ReturnToSelectCardButton.SetActive(false);

        //PreSelectedList = new List<CardSource>();
        PreSelectedHandCardList = new List<HandCard>();

        SelectedList = new List<CardSource>();

        SelectedIndex = new List<int>();

        HandCards = new List<HandCard>();

        MessageText.text = Message;

        CanTargetCondition = _CanTargetCondition;

        CanTargetCondition_ByPreSelecetedList = _CanTargetCondition_ByPreSelecetedList;

        CanEndSelectCondition = _CanEndSelectCondition;

        MaxCount = _MaxCount;

        CanEndNotMax = _CanEndNotMax;

        CanNoSelect = _CanNoSelect;

        if(!GManager.instance.turnStateMachine.DoseStartGame)
        {
            CountText.text = "";
        }

        else
        {
            if(CanEndNotMax)
            {
                if (MaxCount > 1)
                {
                    CountText.text = $"Select up to {MaxCount} cards.";
                }

                else
                {
                    CountText.text = $"Select up to {MaxCount} card.";
                }
            }

            else
            {
                if(MaxCount > 1)
                {
                    CountText.text = $"Select {MaxCount} cards.";
                }

                else
                {
                    CountText.text = $"Select {MaxCount} card.";
                }
            }
        }
        #endregion

        yield return StartCoroutine(OpenSelectCardPanelCoroutine(RootCardSources, CanLookReverseCard, skillInfos));

        CheckSelection();

        yield return new WaitWhile(() => !IsEndSelection);
        SetIsEndSelection(false);
    }

    #region パネルを開いてカードプレハブを生成(カードを基準に)
    IEnumerator OpenSelectCardPanelCoroutine(List<CardSource> RootCardSources, bool CanLookReverseCard, List<SkillInfo> skillInfos)
    {
        List<CardSource> root = new List<CardSource>();

        foreach (CardSource cardSource in RootCardSources)
        {
            root.Add(cardSource);
        }

        yield return new WaitForSeconds(Time.deltaTime);

        #region 選択しないボタン
        NoSelectButton.SetActive(CanNoSelect());
        #endregion

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

                #region 左クリック
                else if (Input.GetMouseButtonUp(0))
                {
                    handCard.OnClickAction?.Invoke(handCard);
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

            handCard.AddClickTarget(OnClickHandCard);

            HandCards.Add(handCard);
        }

        yield return new WaitWhile(() => scrollRect.content.childCount < root.Count);
        yield return new WaitWhile(() => scrollRect.content.childCount != HandCards.Count);

        yield return new WaitForSeconds(Time.deltaTime * 2);

        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            scrollRect.content.GetChild(i).localScale = new Vector3(2.7f, 2.7f, 1);
        }
        #endregion

        CheckSelection();

        Parent.SetActive(true);

        ReturnToSelectCardButton.SetActive(false);

        scrollRect.horizontalNormalizedPosition = 0;

        yield return new WaitForSeconds(Time.deltaTime * 1f);

        if (skillInfos != null)
        {
            foreach(HandCard handCard in HandCards)
            {
                handCard.SetSkillName(skillInfos[HandCards.IndexOf(handCard)].cardEffect);
            }
        }
    }
    #endregion

    #region 左クリック時の処理
    public void OnClickHandCard(HandCard handCard)
    {
        if (CanTargetCondition(handCard.cardSource))
        {
            if (PreSelectedHandCardList.Contains(handCard))
            {
                PreSelectedHandCardList.Remove(handCard);
                SelectedIndex.Remove(HandCards.IndexOf(handCard));
            }

            else
            {
                if (CanTargetCondition_ByPreSelecetedList != null)
                {
                    List<CardSource> _PreSelectedList = new List<CardSource>();

                    foreach(HandCard handCard1 in PreSelectedHandCardList)
                    {
                        _PreSelectedList.Add(handCard1.cardSource);
                    }

                    if (!CanTargetCondition_ByPreSelecetedList(_PreSelectedList, handCard.cardSource))
                    {
                        return;
                    }
                }

                if (PreSelectedHandCardList.Count < MaxCount)
                {
                    PreSelectedHandCardList.Add(handCard);
                    SelectedIndex.Add(HandCards.IndexOf(handCard));
                }

                else
                {
                    if (PreSelectedHandCardList.Count > 0)
                    {
                        PreSelectedHandCardList.RemoveAt(PreSelectedHandCardList.Count - 1);
                        SelectedIndex.RemoveAt(SelectedIndex.Count - 1);
                        PreSelectedHandCardList.Add(handCard);
                        SelectedIndex.Add(HandCards.IndexOf(handCard));
                    }
                }
            }
            CheckSelection();
        }
    }
    #endregion

    #region 選択終了できるかをUIに反映
    public void CheckSelection()
    {
        EndSelectButton.SetActive(CanEndSelection());

        foreach (HandCard handCard in HandCards)
        {
            handCard.OnRemoveSelect();
            handCard.OffSelectedIndexText();

            if (CanTargetCondition(handCard.cardSource))
            {
                if (PreSelectedHandCardList.Contains(handCard))
                {
                    handCard.SetOrangeOutline();
                    handCard.OnOutline();
                    handCard.SetSelectedIndexText(PreSelectedHandCardList.IndexOf(handCard) + 1);
                }

                else
                {
                    handCard.SetBlueOutline();
                    handCard.OnOutline();

                    if (CanTargetCondition_ByPreSelecetedList != null)
                    {
                        List<CardSource> _PreSelectedList = new List<CardSource>();

                        foreach (HandCard handCard1 in PreSelectedHandCardList)
                        {
                            _PreSelectedList.Add(handCard1.cardSource);
                        }

                        if (!CanTargetCondition_ByPreSelecetedList(_PreSelectedList, handCard.cardSource))
                        {
                            handCard.OnRemoveSelect();
                        }
                    }
                }
            }
        }

    }
    #endregion

    #region 選択終了できるか判定
    bool CanEndSelection()
    {
        /*
        if(CanEndSelectCondition != null)
        {
            if(!CanEndSelectCondition(PreSelectedList))
            {
                return false;
            }
        }

        if (CanEndNotMax)
        {
            return true;
        }

        else
        {
            if (PreSelectedList.Count == MaxCount)
            {
                return true;
            }
        }

        return false;
        */

        List<CardSource> _PreSelectedList = new List<CardSource>();

        foreach (HandCard handCard1 in PreSelectedHandCardList)
        {
            _PreSelectedList.Add(handCard1.cardSource);
        }

        if (CanEndSelectCondition != null)
        {
            if (!CanEndSelectCondition(_PreSelectedList))
            {
                return false;
            }
        }

        if (CanEndNotMax)
        {
            return true;
        }

        else
        {
            if (_PreSelectedList.Count == MaxCount)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region カード選択を終了する
    public void CloseSelectCardPanel()
    {
        this.gameObject.SetActive(false);

        ReturnToSelectCardButton.SetActive(false);
    }
    #endregion

    #region 何も選択しないボタンを押したときの処理
    public void OnClickNotSelectButton()
    {
        SelectedList = new List<CardSource>();
        CloseSelectCardPanel();

        SetIsEndSelection(true);

        ContinuousController.instance.StartCoroutine(OnClickButtonActionCoroutine(OnClickNotSelectButtonAction));
    }
    #endregion

    #region 選択終了ボタンを押したときの処理
    public void OnClickEndSelectButton()
    {
        if (CanEndSelection())
        {
            /*
            SelectedList = new List<CardSource>();

            foreach (CardSource cardSource in PreSelectedList)
            {
                SelectedList.Add(cardSource);
            }

            CloseSelectCardPanel();

            SetIsEndSelection(true);

            ContinuousController.instance.StartCoroutine(OnClickButtonActionCoroutine(OnClickEndSelectButtonAction));
            */

            List<CardSource> _PreSelectedList = new List<CardSource>();

            foreach (HandCard handCard1 in PreSelectedHandCardList)
            {
                _PreSelectedList.Add(handCard1.cardSource);
            }

            SelectedList = new List<CardSource>();

            foreach (CardSource cardSource in _PreSelectedList)
            {
                SelectedList.Add(cardSource);
            }

            CloseSelectCardPanel();

            SetIsEndSelection(true);

            ContinuousController.instance.StartCoroutine(OnClickButtonActionCoroutine(OnClickEndSelectButtonAction));
        }
    }
    

    IEnumerator OnClickButtonActionCoroutine(UnityAction Action)
    {
        yield return new WaitForSeconds(0.1f);

        Action?.Invoke();
    }
    #endregion

    #region 一時的にパネル表示・非表示
    public void OnClickReturnToSelectCardButton()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClickCheckFieldButton()
    {
        this.gameObject.SetActive(false);
        ReturnToSelectCardButton.SetActive(true);
    }
    #endregion

}
