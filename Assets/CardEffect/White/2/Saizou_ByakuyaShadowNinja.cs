using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon;
using Photon.Pun;
using System.Linq;
public class Saizou_ByakuyaShadowNinja : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("リョウマ隊のサイゾウ","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("リョウマ")) > 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        return cardEffects;
    }

    #region 忍術の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDiscardSuppot)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("忍術の紋章", "Ninja Emblem", new List<Cost>() { new DiscardHandCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

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
                                new Command_SelectCommand("Front Raw",() => photonView.RPC("SetIsFront_Ninjutsu",RpcTarget.All,true),0),
                                new Command_SelectCommand("Back Raw",() => photonView.RPC("SetIsFront_Ninjutsu",RpcTarget.All,false),1),
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
                yield return StartCoroutine(new IPlayUnit(card, null, isFront, true, hashtable, true).PlayUnit());
                card.Owner.SupportCards = new List<CardSource>();
            }
        }

        return supportEffects;
    }

    bool isFront = false;
    bool endSelect = false;
    [PunRPC]
    public void SetIsFront_Ninjutsu(bool isFront)
    {
        this.isFront = isFront;
        endSelect = true;
    }
    #endregion
}
