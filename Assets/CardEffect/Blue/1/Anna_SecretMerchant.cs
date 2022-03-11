using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Anna_SecretMerchant : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("アンナ姉妹","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10 * card.Owner.FieldUnit.Count((_unit) => _unit != unit && _unit.Character.UnitNames.Contains("アンナ")), CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                return true;
            }

            return false;
        }

        SuccessSupportClass successSupportClass = new SuccessSupportClass();
        successSupportClass.SetUpSuccessSupportClass((cardSource) => cardSource.UnitNames.Contains("アンナ"), (unit) => unit == card.UnitContainingThisCharacter());
        successSupportClass.SetUpICardEffect("100人のアンナ", "",null, null, -1, false,card);
        cardEffects.Add(successSupportClass);

        CanPlayEvenIfExistSameUnitClass canPlayEvenIfExistSameUnitClass = new CanPlayEvenIfExistSameUnitClass();
        canPlayEvenIfExistSameUnitClass.SetUpCanPlayEvenIfExistSameUnitClass((cardSource) => cardSource == card);
        canPlayEvenIfExistSameUnitClass.SetUpICardEffect("100人のアンナ", "",null, null, -1, false,card);
        cardEffects.Add(canPlayEvenIfExistSameUnitClass);

        return cardEffects;
    }
}