using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon;
using Photon.Pun;


public class Chiki_FantomUtaroid : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ブルームパレスの扉", "Portal to the Oasis", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                isHand = false;
                endSelect = false;

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("チキ") && cardSource.Weapons.Contains(Weapon.Sharp),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to add to hand or\nput on library.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    if (card.Owner.isYou)
                    {
                        GManager.instance.commandText.OpenCommandText("Which do you put the card,to hand or to deck top?");

                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("To Hand",() => photonView.RPC("SetIsHand_Chiki",RpcTarget.All,true),0),
                                new Command_SelectCommand("To Library Top",() => photonView.RPC("SetIsHand_Chiki",RpcTarget.All,false),1),
                            };

                        GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                    }

                    yield return new WaitWhile(() => !endSelect);
                    endSelect = false;

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                    Hashtable _hashtable = new Hashtable();
                    _hashtable.Add("cardEffect", activateClass);

                    if (isHand)
                    {
                        yield return ContinuousController.instance.StartCoroutine(new IAddHandCardFromTrash(cardSource,_hashtable).AddHandCardFromTrash());
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Added Hand Card", true));
                    }

                    else
                    {
                        card.Owner.LibraryCards.Insert(0, cardSource);
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Deck Top Card", true));
                    }

                }
            }
        }

        return cardEffects;
    }

    bool isHand = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsHand_Chiki(bool isHand)
    {
        this.isHand = isHand;
        endSelect = true;
    }

    #region 祈りの紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("祈りの紋章", "Miracle Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
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
                CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
                canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner != card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => canNotCriticalClass);

                yield return null;
            }
        }


        return supportEffects;
    }
    #endregion
}

