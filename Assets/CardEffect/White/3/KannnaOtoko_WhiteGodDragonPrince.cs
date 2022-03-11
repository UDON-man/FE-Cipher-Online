using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KannnaOtoko_WhiteGodDragonPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("神祖竜の血族","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (card.Owner.BondCards.Count >= 7)
                {
                    return true;
                }
            }

            return false;
        }

        ChangePlaceDestroyedUnitClass changePlaceDestroyedUnitClass = new ChangePlaceDestroyedUnitClass();
        changePlaceDestroyedUnitClass.SetUpICardEffect("強すぎる力","", new List<Cost>(), new List<System.Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        changePlaceDestroyedUnitClass.SetUpChangePlaceDestroyedUnitClass((unit) => DestroyMode.Bond,(unit) => unit != unit.Character.Owner.Lord);
        cardEffects.Add(changePlaceDestroyedUnitClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.AttackingUnit != null)
            {
                if(GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}