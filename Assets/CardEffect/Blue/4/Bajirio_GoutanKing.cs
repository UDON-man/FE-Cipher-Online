using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Bajirio_GoutanKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("東西の王", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition);
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
            activateClass[1].SetUpICardEffect("手斧", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Hand Axe";
            }

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
