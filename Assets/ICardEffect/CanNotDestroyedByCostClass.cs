using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotDestroyedByCostClass : ICardEffect, ICanNotDestroyedByCost
{
    Func<Unit, bool> UnitCondition { get; set; }
    public void SetUpCanNotDestroyedByCostClass(Func<Unit, bool> UnitCondition)
    {
        this.UnitCondition = UnitCondition;
    }

    public bool CanNotDestroyedByCost(Unit unit)
    {
        if (unit != null)
        {
            if (UnitCondition(unit))
            {
                return true;
            }
        }

        return false;
    }
}
