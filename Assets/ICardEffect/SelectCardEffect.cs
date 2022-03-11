using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon;
using Photon.Pun;
using System;
using UnityEngine.Events;


public class SelectCardEffect : MonoBehaviourPunCallbacks
{
    public void SetUp(
        Func<CardSource, bool> CanTargetCondition,
        Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList, 
        Func<List<CardSource>, bool> CanEndSelectCondition,
        Func<bool> CanNoSelect,
        Func<CardSource, IEnumerator> SelectCardCoroutine,
        Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine, 
        string Message, 
        int MaxCount,
        bool CanEndNotMax, 
        bool isShowOpponent, 
        Mode mode, 
        Root root, 
        List<CardSource> CustomRootCardList, 
        bool CanLookReverseCard,
        Player SelectPlayer,
        ICardEffect cardEffect)
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
        this.SelectPlayer = SelectPlayer;
        this.cardEffect = cardEffect;
    }

    Func<CardSource, bool> CanTargetCondition { get; set; }
    Func<List<CardSource>, CardSource, bool> CanTargetCondition_ByPreSelecetedList { get; set; }
    Func<List<CardSource>, bool> CanEndSelectCondition { get; set; }
    public virtual Func<bool> CanNoSelect { get; set; }

    List<CardSource> targetCards { get; set; } = new List<CardSource>();

    Func<CardSource,IEnumerator> SelectCardCoroutine { get; set; }

    Func<List<CardSource>, IEnumerator> AfterSelectCardCoroutine { get; set; }

    string Message { get; set; }

    int MaxCount { get; set; }

    bool CanEndNotMax { get; set; }

    bool isShowOpponent { get; set; }

    bool CanLookReverseCard { get; set; }

    List<CardSource> CustomRootCardList { get; set; } = new List<CardSource>();

    Player SelectPlayer { get; set; } = null;
    ICardEffect cardEffect { get; set; } = null;

    bool isFront;

    public enum Mode
    {
        AddHand,
        DiscardFromBond,
        Deploy,
        Reverse,
        SetFace,
        PutLibraryTop,
        SetFaceBond,
        AddOrb,
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
    bool endSelect;

    public virtual List<CardSource> RootCardList()
    {
        List<CardSource> RootCardList = new List<CardSource>();

        switch (root)
        {
            case Root.Library:
                foreach (CardSource cardSource in SelectPlayer.LibraryCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Trash:
                foreach (CardSource cardSource in SelectPlayer.TrashCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Bond:
                foreach (CardSource cardSource in SelectPlayer.BondCards)
                {
                    RootCardList.Add(cardSource);
                }
                break;

            case Root.Orb:
                foreach (CardSource cardSource in SelectPlayer.OrbCards)
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
        if (GManager.instance.IsAI && !SelectPlayer.isYou)
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

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(SelectPlayer));

        if (active())
        {
            if (SelectPlayer.isYou)
            {
                GManager.instance.turnStateMachine.IsSelecting = true;

                #region メッセージ表示
                switch (mode)
                {
                    case Mode.AddHand:
                        GManager.instance.commandText.OpenCommandText("Select cards to add to hand.");
                        break;

                    case Mode.DiscardFromBond:
                        GManager.instance.commandText.OpenCommandText("Select cards to discard.");
                        break;

                    case Mode.Deploy:
                        GManager.instance.commandText.OpenCommandText("Select cards to deploy.");
                        break;

                    case Mode.Reverse:
                        GManager.instance.commandText.OpenCommandText("Select cards to reverse.");
                        break;

                    case Mode.SetFace:
                        GManager.instance.commandText.OpenCommandText("Select cards to turn face up.");
                        break;

                    case Mode.PutLibraryTop:
                        GManager.instance.commandText.OpenCommandText("Select cards to place on top of deck.");
                        break;

                    case Mode.SetFaceBond:
                        GManager.instance.commandText.OpenCommandText("Select cards to add to bond.");
                        break;

                    case Mode.AddOrb:
                        GManager.instance.commandText.OpenCommandText("Select cards to add to orb.");
                        break;

                    case Mode.Custom:
                        GManager.instance.commandText.OpenCommandText("Select cards.");
                        break;
                }
                #endregion

                if (root == Root.Library)
                {
                    CanNoSelect = () => true;
                }

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

            #region 選択したカードを表示
            if (isShowOpponent)
            {
                if(targetCards.Count > 0)
                {
                    switch (mode)
                    {
                        case Mode.AddHand:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Added Hand Cards", true));
                            break;

                        case Mode.DiscardFromBond:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Discarded Bond Cards", true));
                            break;

                        case Mode.Deploy:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deployed Cards", true));
                            break;

                        case Mode.Reverse:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Flipped Cards", true));
                            break;

                        case Mode.SetFace:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Face up Cards", true));
                            break;

                        case Mode.PutLibraryTop:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Deck Top Cards", true));
                            break;

                        case Mode.SetFaceBond:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Added Bond Cards", true));
                            break;

                        case Mode.AddOrb:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Added Orb Cards", true));
                            break;

                        case Mode.Custom:
                            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetCards, "Selected Cards", true));
                            break;
                    }
                }
            }
            #endregion

            Hashtable hashtable = new Hashtable();
            hashtable.Add("cardEffect", cardEffect);

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
                        if(mode != Mode.Reverse && mode != Mode.SetFace && mode != Mode.Custom)
                        {
                            cardSource.Owner.BondCards.Remove(cardSource);
                        }
                        break;

                    case Root.Trash:
                        cardSource.Owner.TrashCards.Remove(cardSource);
                        break;
                }

                switch (mode)
                {
                    case Mode.AddHand:
                        cardSource.SetFace();

                        if (root == Root.Trash)
                        {
                            yield return ContinuousController.instance.StartCoroutine(new IAddHandCardFromTrash(cardSource, hashtable).AddHandCardFromTrash());
                        }

                        else
                        {
                            yield return ContinuousController.instance.StartCoroutine(CardObjectController.AddHandCard(cardSource, false));
                        }
                        break;

                    case Mode.DiscardFromBond:
                        cardSource.SetFace();
                        CardObjectController.AddTrashCard(cardSource);
                        break;

                    case Mode.Deploy:
                        cardSource.SetFace();
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
                        cardSource.SetReverse();
                        break;

                    case Mode.SetFace:
                        cardSource.SetFace();
                        break;

                    case Mode.PutLibraryTop:
                        cardSource.SetFace();
                        cardSource.Owner.LibraryCards.Insert(0, cardSource);
                        break;

                    case Mode.SetFaceBond:
                        if(cardSource.CanSetBondThisCard)
                        {
                            cardSource.SetFace();
                            yield return StartCoroutine(new ISetBondCard(cardSource, true).SetBond());
                            yield return StartCoroutine(cardSource.Owner.bondObject.SetBond_Skill(cardSource.Owner));
                        }
                        break;

                    case Mode.AddOrb:
                        yield return StartCoroutine(CardObjectController.AddOrbCard(cardSource,false));
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

        yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(SelectPlayer));
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