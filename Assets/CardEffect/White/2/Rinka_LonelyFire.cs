using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Rinka_LonelyFire : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("鬼神の一撃", "Strike of the Demon", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

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