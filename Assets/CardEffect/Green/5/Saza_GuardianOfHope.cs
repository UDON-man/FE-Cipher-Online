using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;

public class Saza_GuardianOfHope : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("瞬殺", "Bane",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit.Character.Owner != card.Owner)
                                    {
                                        if(GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit.Character.Owner != card.Owner)
                                    {
                                        if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                                        {
                                            Hashtable hashtable1 = new Hashtable();
                                            hashtable1.Add("cardEffect", activateClass);
                                            hashtable1.Add("Unit", new Unit(Unit.Characters));
                                            yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(Unit, 1, BreakOrbMode.Hand, hashtable1).Destroy());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly||timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("碧の護衛", "Green Shadow of Protection", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if(timing == EffectTiming.OnDestroyDuringBattleAlly)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }
                    }

                    else if(timing == EffectTiming.OnDestroyedAnyone)
                    {
                        if (hashtable != null)
                        {
                            if (hashtable.ContainsKey("cardEffect"))
                            {
                                if (hashtable["cardEffect"] is ICardEffect)
                                {
                                    ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];

                                    if (cardEffect.card() != null)
                                    {
                                        if (cardEffect.card() == card)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                doDiscard = false;
                endSelect = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(new IShowLibraryCard(new List<CardSource>() { cardSource }, hashtable, false).ShowLibraryCard());

                    if (card.Owner.isYou)
                    {
                        GManager.instance.commandText.OpenCommandText("Do you discard the card?");

                        List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                        {
                            new Command_SelectCommand("Discard",() => photonView.RPC("SetDoDiscard",RpcTarget.All,true),0),
                            new Command_SelectCommand("Not Discard",() => photonView.RPC("SetDoDiscard",RpcTarget.All,false),1),
                        };

                        yield return GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);
                    }

                    else
                    {
                        GManager.instance.commandText.OpenCommandText("The opponent is selecting if discard.");
                    }

                    yield return new WaitWhile(() => !endSelect);
                    endSelect = false;

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);

                    if (doDiscard)
                    {
                        yield return ContinuousController.instance.StartCoroutine(cardSource.cardOperation.DiscardFromLibrary());

                        GManager.instance.commandText.OpenCommandText("The card was discarded.");
                    }

                    else
                    {
                        GManager.instance.commandText.OpenCommandText("The card was not discarded.");
                    }

                    yield return new WaitForSeconds(2f);
                    GManager.instance.GetComponent<Effects>().OffShowCard();

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
                }
            }
        }

        return cardEffects;
    }

    bool doDiscard = false;
    bool endSelect = false;
    [PunRPC]
    public void SetDoDiscard(bool doDiscard)
    {
        this.doDiscard = doDiscard;
        endSelect = true;
    }
}