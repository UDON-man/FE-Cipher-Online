using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lex_LordlingOfDozel : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("斧戦士の血", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 6)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}