using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AddHasCCClass : ICardEffect, IAddHasCCEffect
{
    Func<CardSource, bool> CardSourceCondition { get; set; }
    Func<Unit, bool> TargetUnitCondition { get; set; }
    public void SetUpAddHasCCClass(Func<CardSource, bool> CardSourceCondition, Func<Unit, bool> TargetUnitCondition)
    {
        this.CardSourceCondition = CardSourceCondition;
        this.TargetUnitCondition = TargetUnitCondition;
    }

    public bool HasCC(CardSource cardSource, Unit targetUnit)
    {
        if (CardSourceCondition(cardSource) && TargetUnitCondition(targetUnit))
        {
            return true;
        }

        return false;
    }
}
