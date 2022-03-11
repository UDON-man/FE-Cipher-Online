using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanNotUntapOnStartTurnClass : ICardEffect, ICanNotUntapOnStartTurnEffect
{
    Func<Unit, bool> CanNotUnTapOnStartTurnCondition { get; set; }
    public void SetUpCanNotUntapOnStartTurnClass(Func<Unit, bool> CanNotUnTapOnStartTurnCondition)
    {
        this.CanNotUnTapOnStartTurnCondition = CanNotUnTapOnStartTurnCondition;
    }

    public bool CanNotUntapOnStartTurn(Unit unit)
    {
        if (CanNotUnTapOnStartTurnCondition(unit))
        {
            return true;
        }

        return false;
    }
}