using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using Photon.Pun;
using System.Linq;

public class Mozume_CountryGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("良成長", "Aptitude",new List<Cost>() { new ReverseCost(2,(cardsource) => true)}, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => cardSource.UnitNames.Contains("モズメ"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to stack on top.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(new IPlayUnit(cardSource, card.UnitContainingThisCharacter(), false, true, hashtable, false).PlayUnit());
                }
            }
        }

        return cardEffects;
    }

    #region 抵抗の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("抵抗の紋章", "Resistance Emblem", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.isDestroydeByBattle)
                        {
                            if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                            {
                                if (card.CanPlayAsNewUnit())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.isYou)
                {
                    GManager.instance.commandText.OpenCommandText("Which do you deploy on Front or Back?");

                    List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Teikou",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Teikou",RpcTarget.All,false),1),
                            };

                    GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                }

                yield return new WaitWhile(() => !endSelect);
                endSelect = false;

                GManager.instance.commandText.CloseCommandText();
                yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                card.Owner.SupportHandCard.gameObject.SetActive(false);

                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass_Support);
                yield return StartCoroutine(new IPlayUnit(card, null, isFront, true, hashtable, false).PlayUnit());
                card.Owner.SupportCards = new List<CardSource>();
            }
        }

        return supportEffects;
    }

    bool isFront = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsFront_Teikou(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
    #endregion
}




