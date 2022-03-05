using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class KamuiOnnna_ProceedingTwilight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("望まぬ戦い", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.Enemy.FieldUnit.Count((_unit) => _unit != card.Owner.Lord) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }

   
}


