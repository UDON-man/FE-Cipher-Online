using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon;
using Photon.Pun;
using System;
using UnityEngine.Events;


public class SelectCardEffect : ICardEffect, ActivateICardEffect
{
    public void SetUp(Func<CardSource, bool> CanTargetCondition, Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList, Func<List<CardSource>, bool> CanEndSelectCondition,Func<bool> CanNoSelect, Func<CardSource, IEnumerator> SelectCardCoroutine, Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine, string Message, int MaxCount, bool CanEndNotMax, bool isShowOpponent, Mode mode, Root root, List<CardSource> CustomRootCardList, bool CanLookReverseCard)
    {
        this.CanTargetCondition = CanTargetCondition;
        this.CanTargetCondition_ByPreSelecetedList = CanTargetCondition_ByPreSelecetedList;
        this.CanEndSelectCondition = CanEndSelectCondition;
        this.CanNoSelect = CanNoSelect;
        this.SelectCardCoroutine = SelectCardCoroutine;
        this.AfterSelectCardCoroutine = AfterSelectCardCoroutine;
        this.Message = Message;
        this.MaxCount = MaxCount;
        this.CanEndNotMax = CanEndNotMax;
        this.isShowOpponent = isShowOpponent;
        this.mode = mode;
        this.root = root;
        this.CustomRootCardList = CustomRootCardList;
        this.CanLookReverseCard = CanLookReverseCard;
    }

    Func<CardSource, bool> CanTargetCondition { get; set; }
    Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    Func<List<CardSource>, bool> CanEndSelectCondition { get; set; }
    public virtual Func<bool> CanNoSelect { get; set; }

    List<CardSource> targetCards { get; set; } = new List<CardSource>();

    Func<CardSource,IEnumerator> SelectCardCoroutine;

    Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine;

    string Message;

    int MaxCount;

    bool CanEndNotMax;

    bool isShowOpponent;

    bool CanLookReverseCard;

    List<CardSource> CustomRootCardList = new List<CardSource>();

    bool isFront;

    public enum Mode
    {
        AddHand,
        DiscardFromHand,
        Deploy,
        Reverse,
        PutLibraryTop,
        Custom,
    }

    public Mode mode;

    public enum Root
    {
        Library,
        Trash,
        Bond,
        Orb,
        Custom,
    }

    public Root root;

    public virtual List<CardSource> RootCardList()
    {
        List<CardSource> RootCardList = new List<CardSource>();

        switch (root)
        {
            case Root.Library:
                foreach (CardSource cardSource in card().Owner.LibraryCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Trash:
                foreach (CardSource cardSource in card().Owner.TrashCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Bond:
                foreach (CardSource cardSource in card().Owner.BondCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Orb:
                foreach (CardSource cardSource in card().Owner.OrbCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Custom:
                foreach (CardSource cardSource in CustomRootCardList)
                {
                    RootCardList.Add(cardSource);
                }
                break;
        }

        return RootCardList;
    }

    public bool active()
    {
        if (RootCardList().Count > 0)
        {
            if (root != Root.Library && root != Root.Custom)
            {
                if (RootCardList().Count((cardSource) => CanTargetCondition(cardSource)) > 0)
                {
                    return true;
                }
            }

            else
            {
                return true;
            }
        }

        return false;
    }

    public virtual IEnumerator Activate(Hashtable hash)
    {
        if (GManager.instance.IsAI && !card().Owner.isYou)
        {
            yield break;
        }

        targetCards = new List<CardSource>();

        yield return GManager.instance.photonWaitController.StartWait("SelectCardEffect");

        foreach (Player player in GManager.instance.turnStateMachine.gameContext.Players)
        {
            GManager.instance.turnStateMachine.OffFieldCardTarget(player);
            GManager.instance.turnStateMachine.OffHandCardTarget(player);
        }

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card().Owner));

        if (active())
        {
            if (card().Owner.isYou)
            {
                GManager.instance.turnStateMachine.IsSelecting = true;

                switch (mode)
                {
                    case Mode.AddHand:
                        GManager.instance.commandText.OpenCommandText("Select cards to add to hand.");
                        break;

                    case Mode.DiscardFromHand:
                        GManager.instance.commandText.OpenCommandText("Select cards to discard.");
                        break;

                    case Mode.Deploy:
                        GManager.instance.commandText.OpenCommandText("Select cards to deploy.");
                        break;

                    case Mode.Reverse:
                        GManager.instance.commandText.OpenCommandText("Select cards to reverse.");
                        break;

                    case Mode.PutLibraryTop:
                        GManager.instance.commandText.OpenCommandText("Select cards to place on top of deck.");
                        break;

                    case Mode.Custom:
                        GManager.instance.commandText.OpenCommandText("Select cards.");
                        break;
                }

                if(root == Root.Library)
                {
                    CanNoSelect = () => true;
                }

                //yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(Message, RootCardList(), CanTargetCondition, MaxCount, CanEndNotMax, CanNoSelect, CanLookReverseCard));

                yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(
                    Message: Message,
                    RootCardSources: RootCardList(),
                    _CanTargetCondition: CanTargetCondition,
                    _CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    _CanEndSelectCondition: CanEndSelectCondition,
                    _MaxCount: MaxCount,
                     _CanEndNotMax: CanEndNotMax,
                    _CanNoSelect: CanNoSelect,
                    CanLookReverseCard: CanLookReverseCard,
                    skillInfos: null));

                List<int> targetCardIDs = new List<int>();

                foreach (CardSource selectedCard in GManager.instance.selectCardPanel.SelectedList)
                {
                    targetCardIDs.Add(selectedCard.cardIndex);
                }

                photonView.RPC("SetTargetCard", RpcTarget.All, targetCardIDs.ToArray());
            }

            else
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting cards.");
            }

            yield return new WaitWhile(() => !endSelect);
            endSelect = false;

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

            if(isShowOpponent)
            {
                if(targetCards.Count > 0)
                {
                    switch (mode)
                    {
                        case Mode.AddHand:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Added Hand Cards", true));
                            break;

                        case Mode.DiscardFromHand:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Discarded Cards", true));
                            break;

                        case Mode.Deploy:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deployed Cards", true));
                            break;

                        case Mode.Reverse:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Flipped Cards", true));
                            break;

                        case Mode.PutLibraryTop:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deck Top Cards", true));
                            break;

                        case Mode.Custom:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Selected Cards", true));
                            break;
                    }
                }
            }

            Hashtable hashtable = new Hashtable();
            hashtable.Add("cardEffect", this);

            foreach (CardSource cardSource in targetCards)
            {
                switch (root)
                {
                    case Root.Library:
                        cardSource.Owner.LibraryCards.Remove(cardSource);

                        //デッキをシャッフル
                        yield return ContinuousController.instance.StartCoroutine(CardObjectController.Shuffle(cardSource.Owner));
                        break;

                    case Root.Bond:
                        if(mode != Mode.Reverse)
                        {
                            cardSource.Owner.BondCards.Remove(cardSource);
                        }
                        
                        break;

                    case Root.Trash:
                        cardSource.Owner.TrashCards.Remove(cardSource);
                        break;
                }

                cardSource.SetFace();
                switch (mode)
                {
                    case Mode.AddHand:
                        if(root == Root.Trash)
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IAddHandCardFromTrash(cardSource, hashtable).AddHandCardFromTrash());
                        }

                        else
                        {
                            yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));
                        }
                        break;

                    case Mode.DiscardFromHand:
                        yield return StartCoroutine(cardSource.cardOperation.DiscardFromHand(hashtable));
                        break;

                    case Mode.Deploy:
                        #region 前衛・後衛を指定
                        if (cardSource.Owner.isYou)
                        {
                            GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                            List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Card",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Card",RpcTarget.All,false),1),
                            };

                            GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                        }

                        yield return new WaitWhile(() => !endSelect);
                        endSelect = false;

                        GManager.instance.commandText.CloseCommandText();
                        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                        #endregion

                        yield return StartCoroutine(new IPlayUnit(cardSource, null, isFront, true,hashtable,false).PlayUnit());
                        break;

                    case Mode.Reverse:
                        cardSource.IsReverse = true;
                        break;

                    case Mode.PutLibraryTop:
                        cardSource.Owner.LibraryCards.Insert(0, cardSource);
                        break;

                    case Mode.Custom:
                        if (SelectCardCoroutine != null)
                        {
                            yield return StartCoroutine(SelectCardCoroutine(cardSource));
                        }
                        break;
                }
            }
        }

        if (AfterSelectCardCoroutine != null)
        {
            yield return StartCoroutine(AfterSelectCardCoroutine(targetCards));
        }

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card().Owner));
        GManager.instance.turnStateMachine.IsSelecting = false;
    }

    [PunRPC]
    public void SetTargetCard(int[] CardIDs)
    {
        targetCards = new List<CardSource>();

        foreach (int CardID in CardIDs)
        {
            targetCards.Add(GManager.instance.turnStateMachine.gameContext.ActiveCardList[CardID]);
        }

        endSelect = true;
    }

    [PunRPC]
    public void SetIsFront_Card(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
}