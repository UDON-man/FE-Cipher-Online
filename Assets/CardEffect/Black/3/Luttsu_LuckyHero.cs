using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Luttsu_LuckyHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("強運の塊", "Fortunate Son", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != card.UnitContainingThisCharacter())
                    {
                        return true;
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

        return cardEffects;
    }

    
}

