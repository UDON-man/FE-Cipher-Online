using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nabar_CriticalSwordUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("キルソード", "Killing Edge", new List<Cost>() { new ReverseCost(3, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("孤高の剣士", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit != card.Owner.Lord) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}
