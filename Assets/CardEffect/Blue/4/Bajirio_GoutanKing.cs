using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Bajirio_GoutanKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("東西の王", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (unit == GManager.instance.turnStateMachine.AttackingUnit || unit == GManager.instance.turnStateMachine.DefendingUnit)
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (card.Owner.SupportCards.Count((_cardSource) => _cardSource.UnitNames.Contains("フラヴィア")) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("手斧", "Hand Axe", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
