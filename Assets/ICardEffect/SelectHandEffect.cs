using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon;
using Photon.Pun;
using System;

public class SelectHandEffect : ICardEffect,ActivateICardEffect
{
    public void SetUp(Player SelectPlayer,Func<CardSource, bool> CanTargetCondition, Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList, Func<List<CardSource>, bool> CanEndSelectCondition, int MaxCount, bool CanNoSelect, bool CanEndNotMax,bool isShowOpponent, Func<CardSource, IEnumerator> SelectCardCoroutine, Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine, Mode mode)
    {
        this.SelectPlayer = SelectPlayer;
        this.CanTargetCondition = CanTargetCondition;
        this.CanTargetCondition_ByPreSelecetedList = CanTargetCondition_ByPreSelecetedList;
        this.CanEndSelectCondition = CanEndSelectCondition;
        this.MaxCount = MaxCount;
        this.CanNoSelect = CanNoSelect;
        this.CanEndNotMax = CanEndNotMax;
        this.isShowOpponent = isShowOpponent;
        this.SelectCardCoroutine = SelectCardCoroutine;
        this.AfterSelectCardCoroutine = AfterSelectCardCoroutine;
        this.mode = mode;
    }

    //手札を選択するプレイヤー
    Player SelectPlayer;
    //選択することのできるカードの条件
    Func<CardSource, bool> CanTargetCondition { get; set; }
    //現在の選択リストの状態でそのカードを選択することができるか
    Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    //選択終了することのできる条件(選択終了時点のリストを参照)
    Func<List<CardSource>, bool> CanEndSelectCondition { get; set; }
    //選択する最大枚数
    int MaxCount { get; set; }
    //選択しないことを選べるか
    bool CanNoSelect { get; set; }
    //最大数未満でも選択を終えられるか
    bool CanEndNotMax { get; set; }
    //相手に見せるか
    bool isShowOpponent { get; set; }
    //(Mode.Custom時限定)選択してする処理
    Func<CardSource, IEnumerator> SelectCardCoroutine;
    //選択した後にする処理
    Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine;
    //選択してする処理の分類
    Mode mode;
    public enum Mode
    {
        Discard,
        SetFaceBond,
        PutLibraryTop,
        Deploy,
        Custom
    }

    //選択された手札カードリスト
    public List<CardSource> targetCards { get; set; } = new List<CardSource>();
    //選択しないフラグ
    bool NoSelect;

    bool isFront;

    #region 選択が可能であるか
    public bool active()
    {
        //if (!GManager.instance.IsAI || card.Owner.isYou)
        {
            if(SelectPlayer != null)
            {
                if (SelectPlayer.HandCards.Count((cardSource) => CanTargetCondition(cardSource)) > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    public virtual IEnumerator Activate(Hashtable hash)
    {
        if (active())
        {
            targetCards = new List<CardSource>();

            NoSelect = false;

            yield return GManager.instance.photonWaitController.StartWait("SelectUnitEffect");

            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                GManager.instance.turnStateMachine.OffFieldCardTarget(player);
                GManager.instance.turnStateMachine.OffHandCardTarget(player);
            }

            if (SelectPlayer.isYou)
            {
                GManager.instance.turnStateMachine.IsSelecting = true;

                #region メッセージ表示
                switch (mode)
                {
                    case Mode.Discard:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a card in your hand to discard.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select cards in your hand to discard.");
                        }

                        break;

                    case Mode.SetFaceBond:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a card in your hand to add to bond.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select cards in your hand to add to bond.");
                        }

                        break;

                    case Mode.PutLibraryTop:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a card in your hand to place on top of deck.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select cards in your hand to place on top of deck.");
                        }

                        break;

                    case Mode.Deploy:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a card in your hand to deploy.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select cards in your hand to deploy.");
                        }

                        break;

                    case Mode.Custom:
                        if (MaxCount == 1)
                        {
                            GManager.instance.commandText.OpenCommandText("Select a card in your hand.");
                        }

                        else
                        {
                            GManager.instance.commandText.OpenCommandText("Select cards in your hand.");
                        }

                        break;
                }
                #endregion

                List<CardSource> PreSelectedHandCards = new List<CardSource>();

                foreach (HandCard handCard in SelectPlayer.HandCardObjects)
                {
                    if (CanTargetCondition(handCard.cardSource))
                    {
                        handCard.AddClickTarget(OnClickHandCard);
                    }
                }

                CheckEndSelect();

                #region 手札カードクリック時の処理
                void OnClickHandCard(HandCard handCard)
                {
                    if (PreSelectedHandCards.Contains(handCard.cardSource))
                    {
                        PreSelectedHandCards.Remove(handCard.cardSource);
                    }

                    else
                    {
                        if (CanTargetCondition_ByPreSelecetedList != null)
                        {
                            if (!CanTargetCondition_ByPreSelecetedList(PreSelectedHandCards, handCard.cardSource))
                            {
                                return;
                            }
                        }

                        if (PreSelectedHandCards.Count < MaxCount)
                        {
                            PreSelectedHandCards.Add(handCard.cardSource);
                        }

                        else
                        {
                            if (PreSelectedHandCards.Count > 0)
                            {
                                PreSelectedHandCards.RemoveAt(PreSelectedHandCards.Count - 1);
                                PreSelectedHandCards.Add(handCard.cardSource);
                            }
                        }
                    }

                    CheckEndSelect();
                }
                #endregion

                #region 選択終了可能かどうか判定しUI表示
                void CheckEndSelect()
                {
                    #region 終了できるかによってUI表示
                    if (CanEndSelect(PreSelectedHandCards))
                    {
                        GManager.instance.selectCommandPanel.SetUpCommandButton(new List<Command_SelectCommand>() { new Command_SelectCommand("End Selection", SetTargetCards_RPC, 0) });

                        void SetTargetCards_RPC()
                        {
                            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
                            {
                                foreach (Unit unit in player.FieldUnit)
                                {
                                    unit.ShowingFieldUnitCard.RemoveSelectEffect();
                                    unit.ShowingFieldUnitCard.OffClickTarget();
                                }

                                foreach(HandCard handCard in player.HandCardObjects)
                                {
                                    handCard.RemoveClickTarget();
                                    handCard.OnRemoveSelect();
                                }
                            }

                            List<int> CardIDs = new List<int>();

                            foreach (CardSource cardSource in PreSelectedHandCards)
                            {
                                CardIDs.Add(cardSource.cardIndex);
                            }

                            photonView.RPC("SetTargetCards", RpcTarget.All, CardIDs.ToArray());

                            GManager.instance.BackButton.CloseSelectCommandButton();
                        }
                    }

                    else
                    {
                        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
                    }
                    #endregion

                    #region 選択リストによってカードUI表示
                    foreach (CardSource cardSource in SelectPlayer.HandCards)
                    {
                        cardSource.ShowingHandCard.OnRemoveSelect();
                        cardSource.ShowingHandCard.OffSelectedIndexText();

                        if (CanTargetCondition(cardSource))
                        {
                            if (PreSelectedHandCards.Contains(cardSource))
                            {
                                cardSource.ShowingHandCard.OnSelect();
                                cardSource.ShowingHandCard.SetOrangeOutline();
                                cardSource.ShowingHandCard.SetSelectedIndexText(PreSelectedHandCards.IndexOf(cardSource)+1);
                            }

                            else
                            {
                                cardSource.ShowingHandCard.OnSelect();
                                cardSource.ShowingHandCard.SetBlueOutline();

                                if (CanTargetCondition_ByPreSelecetedList != null)
                                {
                                    if (!CanTargetCondition_ByPreSelecetedList(PreSelectedHandCards, cardSource))
                                    {
                                        cardSource.ShowingHandCard.OnRemoveSelect();
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region 戻るボタン表示
                if (CanNoSelect)
                {
                    GManager.instance.BackButton.OpenSelectCommandButton("Not select", () => { photonView.RPC("SetNoSelectHand", RpcTarget.All); }, 0);
                }
                #endregion
            }

            else
            {
                #region メッセージ表示
                if (MaxCount == 1)
                {
                    GManager.instance.commandText.OpenCommandText("The opponent is selecting a card.");
                }

                else
                {
                    GManager.instance.commandText.OpenCommandText("The opponent is selecting cards.");
                }
                #endregion

                #region AI
                if (GManager.instance.IsAI)
                {
                    List<CardSource> ValidCards = new List<CardSource>();

                    foreach(CardSource cardSource in SelectPlayer.HandCards)
                    {
                        if(CanTargetCondition(cardSource))
                        {
                            ValidCards.Add(cardSource);
                        }
                    }

                    if(CanEndNotMax)
                    {
                        NoSelect = true;

                        for (int maxCount = 0; maxCount < MaxCount; maxCount++)
                        {
                            IList<int> indexList = Enumerable.Range(0, ValidCards.Count).ToList();

                            if (ValidCards.Count >= maxCount)
                            {
                                for (int i = 0; i < 1000; i++)
                                {
                                    List<int> GetIndexes = indexList.GetRandom(maxCount).ToList();

                                    List<CardSource> GetCards = new List<CardSource>();

                                    foreach (int index in GetIndexes)
                                    {
                                        GetCards.Add(ValidCards[index]);
                                    }

                                    if (CanEndSelect(GetCards))
                                    {
                                        List<int> CardIDs = new List<int>();

                                        foreach (CardSource cardSource in GetCards)
                                        {
                                            CardIDs.Add(cardSource.cardIndex);
                                        }

                                        SetTargetCards(CardIDs.ToArray());
                                        break;
                                    }
                                }
                            }
                        }

                        endSelect = true;
                    }

                    else
                    {
                        IList<int> indexList = Enumerable.Range(0, ValidCards.Count).ToList();

                        NoSelect = true;

                        if (ValidCards.Count >= MaxCount)
                        {
                            for(int i=0;i<1000;i++)
                            {
                                List<int> GetIndexes = indexList.GetRandom(MaxCount).ToList();

                                List<CardSource> GetCards = new List<CardSource>();

                                foreach (int index in GetIndexes)
                                {
                                    GetCards.Add(ValidCards[index]);
                                }

                                if (CanEndSelect(GetCards))
                                {
                                    List<int> CardIDs = new List<int>();

                                    foreach (CardSource cardSource in GetCards)
                                    {
                                        CardIDs.Add(cardSource.cardIndex);
                                    }

                                    SetTargetCards(CardIDs.ToArray());
                                    break;
                                }
                            }
                        }

                        endSelect = true;

                    }
                }
                #endregion
            }

            #region 終了出来るか判定
            bool CanEndSelect(List<CardSource> PreSelectedHandCards)
            {
                //枚数の条件を満たさない場合
                if (!(PreSelectedHandCards.Count == MaxCount || (PreSelectedHandCards.Count <= MaxCount && CanEndNotMax)))
                {
                    return false;
                }

                //指定された条件を満たさない場合
                if (CanEndSelectCondition != null)
                {
                    if (!CanEndSelectCondition(PreSelectedHandCards))
                    {
                        return false;
                    }
                }

                return true;
            }
            #endregion

            //選択終了まで待機
            yield return new WaitWhile(() => !endSelect);
            endSelect = false;

            #region リセット
            foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
            {
                GManager.instance.turnStateMachine.OffFieldCardTarget(player);
                GManager.instance.turnStateMachine.OffHandCardTarget(player);

                foreach (Unit unit in player.FieldUnit)
                {
                    unit.ShowingFieldUnitCard.RemoveSelectEffect();
                }

                foreach(HandCard handCard in player.HandCardObjects)
                {
                    handCard.RemoveClickTarget();
                    handCard.OnRemoveSelect();
                    handCard.OffSelectedIndexText();
                }
            }

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

            #endregion

            if (!NoSelect)
            {
                if (targetCards.Count > 0)
                {
                    if (isShowOpponent||card().Owner.isYou)
                    {
                        switch(mode)
                        {
                            case Mode.Discard:
                                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Discarded Cards", true));

                                break;

                            case Mode.SetFaceBond:
                                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Added Bond Cards", true));

                                break;

                            case Mode.PutLibraryTop:
                                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deck Top Cards", true));

                                break;

                            case Mode.Deploy:
                                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deployed Cards", true));

                                break;

                            case Mode.Custom:
                                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Selected Cards", true));

                                break;
                        }
                    }
                }
                    
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", this);

                #region 選択されたカードに対して処理を行う
                foreach (CardSource cardSource in targetCards)
                {
                    //色毎にエフェクト
                    //GManager.instance.GetComponent<Effects>().CreatePlayerDamageEffect(fieldUnitCard.transform.position + new Vector3(0, 0.15f, 0), card.cEntity_Base.cardColor);

                    switch (mode)
                    {
                        case Mode.Discard:

                            yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));
                            break;

                        case Mode.SetFaceBond:

                            yield return StartCoroutine(cardSource.cardOperation.SetBondFromHand(true));
                            yield return StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                            break;

                        case Mode.PutLibraryTop:
                            yield return StartCoroutine(cardSource.cardOperation.PutLibraryTopFromHand());
                            break;

                        case Mode.Deploy:

                            #region 前衛・後衛を指定
                            if (cardSource.Owner.isYou)
                            {
                                GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                                List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                                {
                                  new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Hand",RpcTarget.All,true),0),
                                  new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Hand",RpcTarget.All,false),1),
                                };

                                GManager.instance.selectCommandPanel.Off();
                                GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                            }

                            yield return new WaitWhile(() => !endSelect);
                            endSelect = false;

                            GManager.instance.commandText.CloseCommandText();
                            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
                            #endregion

                            yield return StartCoroutine(GManager.instance.GetComponent<Effects>().DeleteHandCardEffectCoroutine(cardSource));
                            yield return StartCoroutine(GManager.instance.GetComponent<Effects>().ShowUseHandCardEffect_PlayUnit(cardSource));
                            yield return StartCoroutine(new IPlayUnit(cardSource, null, isFront, true,hashtable,false).PlayUnit());
                            break;

                        case Mode.Custom:
                            if (SelectCardCoroutine != null)
                            {
                                yield return StartCoroutine(SelectCardCoroutine(cardSource));
                            }
                            break;
                    }
                }
                #endregion
                
            }

            if (AfterSelectCardCoroutine != null)
            {
                yield return StartCoroutine(AfterSelectCardCoroutine(targetCards));
            }

        }

        GManager.instance.turnStateMachine.IsSelecting = false;
    }

    #region カード選択を決定
    [PunRPC]
    public void SetTargetCards(int[] CardIDs)
    {
        targetCards = new List<CardSource>();

        foreach(int CardID in CardIDs)
        {
            targetCards.Add(GManager.instance.turnStateMachine.gameContext.ActiveCardList[CardID]);
        }

        NoSelect = false;

        endSelect = true;
    }
    #endregion

    #region 何も選ばない
    [PunRPC]
    public void SetNoSelectHand()
    {
        GManager.instance.selectCommandPanel.CloseSelectCommandPanel();

        targetCards = new List<CardSource>();

        NoSelect = true;

        endSelect = true;
    }
    #endregion

    [PunRPC]
    public void SetIsFront_Hand(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}

public static class ListExtension
{
    public static IEnumerable<T> GetRandom<T>(this IList<T> list, int count)
    {
        var random = new System.Random();

        var indexList = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            indexList.Add(i);
        }

        for (int i = 0; i < count; i++)
        {
            int index = random.Next(0, indexList.Count);
            int value = indexList[index];
            indexList.RemoveAt(index);
            yield return list[value];
        }
    }
}
