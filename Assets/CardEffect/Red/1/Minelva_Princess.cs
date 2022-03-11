using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Minelva_Princess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("飛竜の鞭", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * card.Owner.FieldUnit.Count((otherUnit) => otherUnit != card.UnitContainingThisCharacter() && otherUnit.Weapons.Contains(Weapon.Wing)), CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                return true;
            }

            return false;
        }

        InvalidationClass invalidationClass = new InvalidationClass();
        invalidationClass.SetUpICardEffect("アイオテの盾","", null, null,- 1, false,card);
        invalidationClass.SetUpInvalidationClass((cardEffect) => cardEffect.EffectName == "飛行特効");
        cardEffects.Add(invalidationClass);

        return cardEffects;
    }
}