using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanNotDestroyedByBattleClass : ICardEffect, ICanNotDestroyedByBattle
{
    Func<Unit, bool> AttackingUnitCondition { get; set; }
    Func<Unit, bool> DefendingUnitCondition { get; set; }
    public void SetUpCanNotDestroyedByBattleClass(Func<Unit, bool> AttackingUnitCondition, Func<Unit, bool> DefendingUnitCondition)
    {
        this.AttackingUnitCondition = AttackingUnitCondition;
        this.DefendingUnitCondition = DefendingUnitCondition;
    }

    public bool CanNotDestroyedByBattle(Unit AttackingUnit, Unit DefendingUnit)
    {
        if (AttackingUnitCondition(AttackingUnit) && DefendingUnitCondition(DefendingUnit))
        {
            return true;
        }

        return false;
    }
}
