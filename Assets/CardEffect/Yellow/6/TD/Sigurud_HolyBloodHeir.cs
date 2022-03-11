using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Sigurud_HolyBloodHeir : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("聖騎士の記章", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (unit.Character != null && IsExistOnField(null, card))
                {
                    if (card.Owner.GetFrontUnits().Contains(unit) && unit != card.UnitContainingThisCharacter() && unit.Character.Owner == card.Owner)
                    {
                        if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.UnitNames.Contains("シグルド")) >= 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("禁じられた邂逅", "", null, null, -1, false, card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition1, true);
        cardEffects.Add(powerUpClass1);

        bool PowerUpCondition1(Unit unit)
        {
            if (unit.Character != null && IsExistOnField(null, card))
            {
                if (unit == card.UnitContainingThisCharacter())
                {
                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetFrontUnits().Count((_unit) => _unit.Character.UnitNames.Contains("ディアドラ")) > 0)
                        {
                            return true;
                        }
                    }

                    else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        if (card.Owner.GetBackUnits().Count((_unit) => _unit.Character.UnitNames.Contains("ディアドラ")) > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}
