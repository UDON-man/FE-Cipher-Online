using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Harold_MrHardLock : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("ヒーローの心得", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }
            }

            return false;
        }

        StrikeUpClass strikeUpClass = new StrikeUpClass();
        strikeUpClass.SetUpICardEffect("助けて! ハロルド!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(strikeUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.OrbCards.Count < card.Owner.Enemy.OrbCards.Count)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}