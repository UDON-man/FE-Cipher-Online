using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Challotte_HasDoubleFace : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("夢は玉の輿", "", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + card.Owner.Enemy.FieldUnit.Count((_unit) => _unit.Character.sex.Contains(Sex.female)), (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}