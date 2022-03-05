using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Rinka_LonelyFire : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("鬼神の一撃", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        if (ContinuousController.instance.language == Language.ENG)
        {
            activateClass[0].EffectName = "Strike of the Demon";
        }

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                return true;
            }

            return false;
        }

        int ChangePower(Unit unit,int Power)
        {
            if(unit != null)
            {
                if (unit.IsClassChanged())
                {
                    Power += 40;
                }

                else
                {
                    Power += 20;
                }
            }

            return Power;
        }

        return cardEffects;
    }
}