using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Yashiro_KnowBond : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("謎の転校生", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if(unit.Character.Owner == card.Owner)
            {
                if (unit == card.UnitContainingThisCharacter() || unit.Character.UnitNames.Contains("ナバール"))
                {
                    return true;
                }
            }

            return false;
        }

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && !_unit.Character.UnitNames.Contains("ナバール")) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

