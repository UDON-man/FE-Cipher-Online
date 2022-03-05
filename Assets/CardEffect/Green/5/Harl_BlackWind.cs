using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Harl_BlackWind : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass1 = new PowerUpClass();
        powerUpClass1.SetUpICardEffect("目覚めし竜牙", null, null, -1, false);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(powerUpClass1);

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("手斧", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

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
