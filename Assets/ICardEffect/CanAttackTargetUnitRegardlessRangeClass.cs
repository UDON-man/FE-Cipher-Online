using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanAttackTargetUnitRegardlessRangeClass : ICardEffect, ICanAttackTargetUnitRegardlessRangeEffect
{
    Func<Unit, bool> AttackingUnitCondition { get; set; }
    Func<Unit, bool> DefendingUnitCondition { get; set; }
    public void SetUpCanAttackTargetUnitRegardlessRangeClass(Func<Unit, bool> AttackingUnitCondition, Func<Unit, bool> DefendingUnitCondition)
    {
        this.AttackingUnitCondition = AttackingUnitCondition;
        this.DefendingUnitCondition = DefendingUnitCondition;
    }

    public bool CanAttackTargetUnitRegardlessRange(Unit AttackingUnit, Unit DefendingUnit)
    {
        if (AttackingUnitCondition(AttackingUnit) && DefendingUnitCondition(DefendingUnit))
        {
            return true;
        }

        return false;
    }
}
