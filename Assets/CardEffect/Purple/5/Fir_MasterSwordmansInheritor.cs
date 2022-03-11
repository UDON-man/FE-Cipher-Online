using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fir_MasterSwordmansInheritor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
        canNotBeEvadedClass.SetUpICardEffect("共鳴する刃","",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 },-1,false,card);
        canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit != card.UnitContainingThisCharacter() && AttackingUnit.Character.Owner == card.Owner, (DefendingUnit) => DefendingUnit != DefendingUnit.Character.Owner.Lord);
        cardEffects.Add(canNotBeEvadedClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if(card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("フィル")) > 0)
            {
                return true;
            }

            return false;
        }

        if (timing == EffectTiming.BeforeDiscardCritical_EvasionCard)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("剣姫の逆鱗", "Sword Priness's Imperial Wrath", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                bool endSelect = false;
                GManager.instance.commandText.OpenCommandText("Do you place the card on top of deck?");

                List<Command_SelectCommand> command_SelectCommands = new List<Command_SelectCommand>()
                            {
                                new Command_SelectCommand("Place Deck Top",() => { GManager.instance.GetComponent<Critical_Evasion>().handmode = SelectHandEffect.Mode.PutLibraryTop; endSelect = true; },0),
                                new Command_SelectCommand("Discard",() => { GManager.instance.GetComponent<Critical_Evasion>().handmode = SelectHandEffect.Mode.Discard; endSelect = true; },1),
                            };

                GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);

                yield return new WaitWhile(() => !endSelect);
                endSelect = false;
            }
        }

        return cardEffects;
    }

    #region 剣聖の一喝
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("剣聖の一喝", "Master Swordman's Roar", new List<Cost>() { new ReverseCost(1,(cardSource) => true)}, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
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
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(CardObjectController.MissSupport(card.Owner.Enemy));
            }
        }

        return supportEffects;
    }
    #endregion
}
