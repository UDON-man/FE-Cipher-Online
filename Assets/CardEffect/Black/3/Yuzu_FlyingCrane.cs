using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Yuzu_FlyingCrane : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("禁じられた秘剣", "Forbidden Sword", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);

                ActivateClass activateClass1 = new ActivateClass();
                activateClass1.SetUpICardEffect("このユニットを\n撃破する", "Destroy\nthis unit.", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                activateClass1.SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(UntilTurnEndAction);

                bool CanUseCondition(Hashtable hashtable1)
                {
                    if (card.UnitContainingThisCharacter() != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            return true;
                        }
                    }

                    return false;
                }

                IEnumerator ActivateCoroutine1()
                {
                    Hashtable hashtable1 = new Hashtable();
                    hashtable1.Add("cardEffect", activateClass1);
                    hashtable1.Add("Unit", new Unit(card.UnitContainingThisCharacter().Characters));
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(card.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand, hashtable1).Destroy());
                }

                ICardEffect UntilTurnEndAction(EffectTiming _timing)
                {
                    if (_timing == EffectTiming.OnEndAttackAnyone)
                    {
                        return activateClass1;
                    }

                    return null;
                }

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("不屈の精神","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.OrbCards.Count <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

