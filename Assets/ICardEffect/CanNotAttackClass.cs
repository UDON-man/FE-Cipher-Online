using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotAttackClass : ICardEffect,ICanNotAttackTargetUnitCardEffect
{
    Func<Unit, bool> AttackerCondition { get; set; }
    Func<Unit, bool> DefenderCondition { get; set; }
    public void SetUpCanNotAttackClass(Func<Unit,bool> AttackerCondition,Func<Unit,bool> DefenderCondition)
    {
        this.AttackerCondition = AttackerCondition;
        this.DefenderCondition = DefenderCondition;
    }

    public bool CanNotAttackTargetUnit(Unit Attacker, Unit Defender)
    {
        if(AttackerCondition != null && DefenderCondition != null && Attacker != null && Defender != null)
        {
            if(AttackerCondition(Attacker) && DefenderCondition(Defender))
            {
                return true;
            }
        }

        return false;
    }
}
