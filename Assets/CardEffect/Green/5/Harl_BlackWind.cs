using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Harl_BlackWind : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("目覚めし竜牙","", null, null, -1, false,card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter() && GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner, true);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 3);
        cardEffects.Add(powerUpClass1);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("手斧", "Hand Axe",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
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
