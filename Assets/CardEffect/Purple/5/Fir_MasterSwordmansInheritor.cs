using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fir_MasterSwordmansInheritor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
        canNotBeEvadedClass.SetUpICardEffect("共鳴する刃",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 },-1,false);
        canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit != this.card.UnitContainingThisCharacter() && AttackingUnit.Character.Owner == card.Owner, (DefendingUnit) => DefendingUnit != DefendingUnit.Character.Owner.Lord);
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
            activateClass[0].SetUpICardEffect("剣姫の逆鱗", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Sword Priness's Imperial Wrath";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable))
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
                                new Command_SelectCommand("Place Deck Top",() => { GManager.instance.GetComponent<Critical_Evasion>().handmode = SelectCardEffect.Mode.PutLibraryTop; endSelect = true; },0),
                                new Command_SelectCommand("Discard",() => { GManager.instance.GetComponent<Critical_Evasion>().handmode = SelectCardEffect.Mode.DiscardFromHand;endSelect = true; },1),
                            };

                GManager.instance.selectCommandPanel.SetUpCommandButton(command_SelectCommands);

                yield return new WaitWhile(() => !endSelect);
                endSelect = false;
            }
        }

        return cardEffects;
    }

    #region 剣聖の一喝
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("剣聖の一喝", new List<Cost>() { new ReverseCost(1,(cardSource) => true)}, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Master Swordman's Roar";
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

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(CardObjectController.MissSupport(card.Owner.Enemy));
            }
        }


        return supportEffects;
    }
    #endregion
}
