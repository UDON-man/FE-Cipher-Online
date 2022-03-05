using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanLevelUpToThisCardClass : ICardEffect, ICanLevelUpToThisCardEffect
{
    Func<CardSource, bool> CardSourceCondition { get; set; }
    Func<Unit, bool> TargetUnitCondition { get; set; }
    public void SetUpCanLevelUpToThisCardClass(Func<CardSource, bool> CardSourceCondition, Func<Unit, bool> TargetUnitCondition)
    {
        this.CardSourceCondition = CardSourceCondition;
        this.TargetUnitCondition = TargetUnitCondition;
    }


    public bool CanLevelUpToThisCard(CardSource cardSource, Unit targetUnit)
    {
        if (CardSourceCondition(cardSource) && TargetUnitCondition(targetUnit))
        {
            return true;
        }

        return false;
    }
}

