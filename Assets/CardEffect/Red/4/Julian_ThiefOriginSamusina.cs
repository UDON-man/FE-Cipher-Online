using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using Photon.Pun;
public class Julian_ThiefOriginSamusina : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            activateClass[0].SetUpICardEffect("潜入調査", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Investigative Infiltration";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
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
                doDiscard = false;
                endSelect = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[0]);
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

    #region 盗賊の紋章

    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpICardEffect("盗賊の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support[0]);

            IEnumerator ActivateCoroutine()
            {
                doDiscard = false;
                endSelect = false;

                yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner.Enemy));

                if (card.Owner.Enemy.LibraryCards.Count > 0)
                {
                    CardSource cardSource = card.Owner.Enemy.LibraryCards[0];

                    //ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Library Card", false));

                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass_Support[0]);
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
