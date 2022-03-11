using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Jikbelt_RunHollyKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("覇道を継ぐ者", "Successor of Militia", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit != DefendingUnit.Character.Owner.Lord);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);

                yield return null;
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("次世代の絆","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.cardColors.Contains(CardColor.White)) > 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}

