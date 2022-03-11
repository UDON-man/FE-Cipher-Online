using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanNotMoveClass : ICardEffect, ICanNotMoveEffect
{
    Func<Unit, bool> CanNotMoveCondition { get; set; }
    public void SetUpCanNotMoveClass(Func<Unit, bool> CanNotMoveCondition)
    {
        this.CanNotMoveCondition = CanNotMoveCondition;
    }

    public bool CanNotMove(Unit unit)
    {
        if (CanNotMoveCondition(unit))
        {
            return true;
        }

        return false;
    }
}