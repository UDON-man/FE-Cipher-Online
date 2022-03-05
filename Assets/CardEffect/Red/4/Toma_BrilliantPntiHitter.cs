using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Toma_BrilliantPntiHitter : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("ガイアの囁き", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 60, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (unit == GManager.instance.turnStateMachine.AttackingUnit || unit == GManager.instance.turnStateMachine.DefendingUnit)
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (card.Owner.SupportCards.Count((_cardSource) => _cardSource.UnitNames.Contains("カイン")) > 0)
                    {
                        if(GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
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
