using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Ema_FromTodayFalcon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("ビギナーズ・ラック!", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count + card.Owner.Enemy.FieldUnit.Count == 7)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}

