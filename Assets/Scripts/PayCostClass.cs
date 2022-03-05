using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System.Linq;
using System;
public class PayCostClass : MonoBehaviourPunCallbacks
{
    bool endSelect = false;

    #region リバースコストの支払い
    List<CardSource> targetReverseCards = new List<CardSource>();
    public IEnumerator PayReverseCost(int reverseCount,Func<CardSource,bool> CanTargetCondition,CardSource card)
    {
        targetReverseCards = new List<CardSource>();

        yield return GManager.instance.photonWaitController.StartWait("PayReverseCost");

        if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && CanTargetCondition(cardSource)) >= reverseCount)
        {
            if (card.Owner.isYou)
            {
                GManager.instance.commandText.OpenCommandText("Select Bond cards to flip.");

                yield return StartCoroutine(GManager.instance.selectCardPanel.OpenSelectCardPanel(
                    Message: "Select Bond cards to flip.",
                    RootCardSources: GManager.instance.You.BondCards,
                    _CanTargetCondition: (cardSource) => !cardSource.IsReverse && CanTargetCondition(cardSource),
                    _CanTargetCondition_ByPreSelecetedList: null,
                    _CanEndSelectCondition: null,
                    _MaxCount:reverseCount,
                     _CanEndNotMax:false,
                    _CanNoSelect:() => false,
                    CanLookReverseCard:true,
                    skillInfos: null));

                List<int> targetCardIDs = new List<int>();

                foreach (CardSource selectedCard in GManager.instance.selectCardPanel.SelectedList)
                {
                    targetCardIDs.Add(selectedCard.cardIndex);
                }

                photonView.RPC("SetTargetReverseCards", RpcTarget.All, targetCardIDs.ToArray());
            }

            else
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting Bond cards to flip.");
            }

            yield return new WaitWhile(() => !endSelect);
            endSelect = false;

            GManager.instance.commandText.CloseCommandText();
            yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

            foreach (CardSource cardSource in targetReverseCards)
            {
                cardSource.IsReverse = true;
            }

            ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(targetReverseCards, "Flipped Bond Cards", false));

            yield return new WaitForSeconds(1.5f);

            GManager.instance.GetComponent<Effects>().OffShowCard();
        }
    }

    [PunRPC]
    public void SetTargetReverseCards(int[] CardIDs)
    {
        targetReverseCards = new List<CardSource>();

        foreach (int CardID in CardIDs)
        {
            targetReverseCards.Add(GManager.instance.turnStateMachine.gameContext.ActiveCardList[CardID]);
        }

        endSelect = true;
    }
    #endregion

    #region タップコストの支払い
    public IEnumerator PayTapCost(CardSource card)
    {
        if (!card.UnitContainingThisCharacter().IsTapped)
        {
            yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().Tap());
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region 手札を捨てる、山札の上に置くコストの支払い
    List<CardSource> SelectedCards = new List<CardSource>();
    public IEnumerator PaySelectHandCost(int SelectCount, Func<CardSource, bool> CanTargetCondition,CardSource card,SelectHandEffect.Mode mode,bool isShowOpponent)
    {
        SelectedCards = new List<CardSource>();
        endSelect = false;

        yield return GManager.instance.photonWaitController.StartWait("SelectCritical_Evade");

        if (card.Owner.isYou)
        {
            if (mode == SelectHandEffect.Mode.Discard)
            {
                GManager.instance.commandText.OpenCommandText("Select cards to discard.");
            }

            else if (mode == SelectHandEffect.Mode.PutLibraryTop)
            {
                GManager.instance.commandText.OpenCommandText("Select cards to place on top of deck.");
            }

            #region 手札のカードに捨てるクリック操作を追加
            List<HandCard> PreSelectHandCard = new List<HandCard>();

            foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
            {
                if (CanTargetCondition(handCard.cardSource))
                {
                    handCard.AddClickTarget(OnClickHandCard);
                }
            }

            CheckEndSelect();

            void OnClickHandCard(HandCard _handCard)
            {
                if (PreSelectHandCard.Contains(_handCard))
                {
                    PreSelectHandCard.Remove(_handCard);
                }

                else
                {
                    if (PreSelectHandCard.Count < SelectCount)
                    {
                        PreSelectHandCard.Add(_handCard);
                    }

                    else
                    {
                        if (PreSelectHandCard.Count > 0)
                        {
                            PreSelectHandCard.RemoveAt(PreSelectHandCard.Count - 1);
                            PreSelectHandCard.Add(_handCard);
                        }
                    }
                }

                CheckEndSelect();
            }

            void CheckEndSelect()
            {
                if (PreSelectHandCard.Count == SelectCount)
                {
                    GManager.instance.selectCommandPanel.SetUpCommandButton(new List<Command_SelectCommand>() { new Command_SelectCommand("Discard", SelectDiscardCard, 0) });

                    void SelectDiscardCard()
                    {
                        foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                        {
                            handCard.RemoveClickTarget();
                        }

                        List<int> cardIDs = new List<int>();

                        foreach (HandCard handCard in PreSelectHandCard)
                        {
                            cardIDs.Add(handCard.cardSource.cardIndex);
                        }

                        photonView.RPC("SetDiscardCard", RpcTarget.All, cardIDs.ToArray());
                    }
                }

                else
                {
                    GManager.instance.selectCommandPanel.CloseSelectCommandPanel();
                }

                foreach (HandCard handCard in GManager.instance.You.HandCardObjects)
                {
                    handCard.OnRemoveSelect();

                    if (CanTargetCondition(handCard.cardSource))
                    {
                        if (PreSelectHandCard.Contains(handCard))
                        {
                            handCard.OnSelect();
                            handCard.SetOrangeOutline();
                        }

                        else
                        {
                            handCard.OnOutline();
                            handCard.SetBlueOutline();
                        }
                    }
                }
            }
            #endregion
        }

        else if (GManager.instance.IsAI)
        {
            if (GManager.instance.IsAI)
            {
                yield break;
            }

            if (mode == SelectHandEffect.Mode.Discard)
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting cards to discard.");
            }

            else if (mode == SelectHandEffect.Mode.Discard)
            {
                GManager.instance.commandText.OpenCommandText("The opponent is selecting cards to place on top of deck.");
            }
            
        }

        yield return new WaitWhile(() => !endSelect);
        endSelect = false;

        if (card.Owner.isYou)
        {
            foreach (HandCard handCard in card.Owner.HandCardObjects)
            {
                handCard.RemoveClickTarget();
                handCard.OnRemoveSelect();
            }
        }

        GManager.instance.commandText.CloseCommandText();
        yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

        if(SelectedCards.Count > 0)
        {
            if (mode == SelectHandEffect.Mode.Discard)
            {
                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(SelectedCards, "Discarded Cards", true));

                #region 選択された手札のカードを捨てる
                foreach (CardSource DiscardCard in SelectedCards)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("isCost", true);
                    yield return StartCoroutine(DiscardCard.cardOperation.DiscardFromHand(hashtable));
                }
                #endregion
            }

            else if (mode == SelectHandEffect.Mode.PutLibraryTop)
            {
                if(card.Owner.isYou||isShowOpponent)
                {
                    ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(SelectedCards, "Deck Top Cards", true));
                }

                #region 選択された手札のカードを山札の上に置く
                foreach (CardSource DiscardCard in SelectedCards)
                {
                    yield return StartCoroutine(DiscardCard.cardOperation.PutLibraryTopFromHand());
                }
                #endregion
            }
        }
    }

    [PunRPC]
    public void SetDiscardCard(int[] cardIDs)
    {
        SelectedCards = new List<CardSource>();

        foreach (int cardID in cardIDs)
        {
            SelectedCards.Add(GManager.instance.turnStateMachine.gameContext.ActiveCardList[cardID]);
        }

        endSelect = true;
    }
    #endregion

    #region 撃破コストの支払い
    public IEnumerator PayDestroyCost(CardSource card)
    {
        yield return StartCoroutine(new IDestroyUnit(card.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand,null).Destroy());
    }
    #endregion

    #region オーブ破壊コストの支払い
    public IEnumerator PayBreakOrbCost(Player player,int BreakCount,BreakOrbMode breakOrbMode)
    {
        yield return ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<BreakOrb>().BreakOrbCoroutine(player, BreakCount, breakOrbMode));
    }
    #endregion

    #region コストを支払う
    public IEnumerator PayCost(List<Cost> costs,CardSource card)
    {
        foreach (Cost cost in costs)
        {
            if (cost != null)
            {
                if (cost is ReverseCost)
                {
                    yield return StartCoroutine(PayReverseCost(((ReverseCost)cost).ReverseCount, ((ReverseCost)cost).CanTargetCondition, card));
                }

                else if (cost is TapCost)
                {
                    yield return StartCoroutine(PayTapCost(card));
                }

                else if(cost is SelectAllyCost)
                {
                    SelectUnitEffect selectUnitEffect = GManager.instance.GetComponent<SelectUnitEffect>();

                    selectUnitEffect.SetUp(
                        SelectPlayer: ((SelectAllyCost)cost).SelectPlayer,
                        CanTargetCondition: ((SelectAllyCost)cost).CanTargetCondition,
                        CanTargetCondition_ByPreSelecetedList: ((SelectAllyCost)cost).CanTargetCondition_ByPreSelecetedList,
                        CanEndSelectCondition: ((SelectAllyCost)cost).CanEndSelectCondition,
                        MaxCount: ((SelectAllyCost)cost).MaxCount,
                        CanNoSelect: ((SelectAllyCost)cost).CanNoSelect,
                        CanEndNotMax: ((SelectAllyCost)cost).CanEndNotMax,
                        SelectUnitCoroutine: ((SelectAllyCost)cost).SelectUnitCoroutine,
                        AfterSelectUnitCoroutine: ((SelectAllyCost)cost).AfterSelectUnitCoroutine,
                        mode: ((SelectAllyCost)cost).mode);

                    yield return StartCoroutine(selectUnitEffect.Activate(null));
                }

                else if (cost is DiscardHandCost)
                {
                    yield return StartCoroutine(PaySelectHandCost(((DiscardHandCost)cost).DiscardCount, ((DiscardHandCost)cost).CanTargetCondition,card, SelectHandEffect.Mode.Discard,true));
                }

                else if (cost is PutHandLibraryTopCost)
                {
                    yield return StartCoroutine(PaySelectHandCost(((PutHandLibraryTopCost)cost).SelectCount, ((PutHandLibraryTopCost)cost).CanTargetCondition, card, SelectHandEffect.Mode.PutLibraryTop, ((PutHandLibraryTopCost)cost).isShowOpponent));
                }

                else if (cost is CustomCost)
                {
                    if (((CustomCost)cost).PayCostCoroutine != null)
                    {
                        yield return StartCoroutine(((CustomCost)cost).PayCostCoroutine);
                    }
                }

                else if(cost is DestroySelfCost)
                {
                    yield return StartCoroutine(PayDestroyCost(card));
                }

                else if(cost is BreakOrbCost)
                {
                    yield return StartCoroutine(PayBreakOrbCost(((BreakOrbCost)cost).player, ((BreakOrbCost)cost).BreakCount, ((BreakOrbCost)cost).breakOrbMode));
                }
            }
        }
    }
    #endregion
}
