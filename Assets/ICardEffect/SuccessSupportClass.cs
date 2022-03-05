using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SuccessSupportClass : ICardEffect, ISuccessSupportTargetUnitCardEffect
{
    Func<CardSource, bool> SupportCardCondition { get; set; }
    Func<Unit, bool> TargetUnitCondition { get; set; }
    public void SetUpSuccessSupportClass(Func<CardSource, bool> SupportCardCondition, Func<Unit, bool> TargetUnitCondition)
    {
        this.SupportCardCondition = SupportCardCondition;
        this.TargetUnitCondition = TargetUnitCondition;
    }

    public bool SuccessSupporTargetUnit(CardSource SupportCard, Unit TargetUnit)
    {
        if (SupportCardCondition(SupportCard) && TargetUnitCondition(TargetUnit))
        {
            return true;
        }

        return false;
    }
}