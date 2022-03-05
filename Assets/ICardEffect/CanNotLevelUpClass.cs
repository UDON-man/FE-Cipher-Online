using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanNotLevelUpClass : ICardEffect, ICanNotLevelUpEffect
{
    Func<Unit, bool> CanNotLevelUpCondition { get; set; }
    public void SetUpCanNotLevelUpCondition(Func<Unit, bool> CanNotLevelUpCondition)
    {
        this.CanNotLevelUpCondition = CanNotLevelUpCondition;
    }

    public bool CanNotLevelUp(Unit unit)
    {
        if (CanNotLevelUpCondition(unit))
        {
            return true;
        }

        return false;
    }
}