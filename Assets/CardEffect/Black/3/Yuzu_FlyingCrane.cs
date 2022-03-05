using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Yuzu_FlyingCrane : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("禁じられた秘剣", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Forbidden Sword";
            }

            IEnumerator ActivateCoroutine()
            {
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == this.card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(canNotBeEvadedClass);

                activateClass[1].SetUpICardEffect("このユニットを\n撃破する", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
                activateClass[1].SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                card.Owner.UntilTurnEndActions.Add(UntilTurnEndAction);

                if (ContinuousController.instance.language == Language.ENG)
                {
                    activateClass[1].EffectName = "Destroy\nthis unit.";
                }

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
                    hashtable1.Add("cardEffect", activateClass[1]);

                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(card.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand, hashtable1).Destroy());
                }

                ActivateICardEffect UntilTurnEndAction(EffectTiming _timing)
                {
                    if (_timing == EffectTiming.OnEndAttackAnyone)
                    {
                        return activateClass[1];
                    }

                    return null;
                }

                yield return null;
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("不屈の精神", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition);
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

