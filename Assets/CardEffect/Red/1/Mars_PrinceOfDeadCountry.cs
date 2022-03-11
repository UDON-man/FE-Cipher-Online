using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mars_PrinceOfDeadCountry : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("英雄の資質","", null, null,-1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition, true);

        bool CanPowerUpCondition(Unit unit)
        {
            if(card.UnitContainingThisCharacter() == unit)
            {
                if(card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Character.cardColors.Contains(CardColor.Red)) >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}

