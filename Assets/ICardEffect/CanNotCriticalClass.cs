using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotCriticalClass : ICardEffect, ICanNotCriticalCardEffect
{
    Func<Unit, bool> CanNotCriticalCondition { get; set; }
    public void SetUpCanNotCriticalClass(Func<Unit, bool> CanNotCriticalCondition)
    {
        this.CanNotCriticalCondition = CanNotCriticalCondition;
    }

    public bool CanNotCritical(Unit unit)
    {
        if(CanNotCriticalCondition(unit))
        {
            return true;
        }

        return false;
    }
}
