using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Tiban_FenikisKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("化身の咆哮_空を統べる者","", null, new List<Func<Hashtable, bool>>(), -1, false,card);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit.Character.Owner == card.Owner, true);
        cardEffects.Add(powerUpClass);

        int ChangePower(Unit unit,int Power)
        {
            if(unit == card.UnitContainingThisCharacter())
            {
                return Power + 10 * card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Weapons.Contains(Weapon.Beast));
            }

            else
            {
                if(unit.Weapons.Contains(Weapon.Beast))
                {
                    if (card.UnitContainingThisCharacter().Power >= 100)
                    {
                        return Power + 10;
                    }
                }
            }

            return Power;
        }

        return cardEffects;
    }
}

