using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class Gaia_SweetNaThief : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("甘味補充", "Sweet Replenishment",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                    {
                        if(!card.UnitContainingThisCharacter().IsTapped)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }

    #region 盗賊の紋章

    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("盗賊の紋章", "Theif Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass_Support);
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

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }


        return supportEffects;
    }

    bool doDiscard = false;
    bool endSelect = false;
    [PunRPC]
    public void SetDoDiscard(bool doDiscard)
    {
        this.doDiscard = doDiscard;
        endSelect = true;
    }
    #endregion
}
