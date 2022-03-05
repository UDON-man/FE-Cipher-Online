using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotSetBondClass : ICardEffect, ICanNotSetBondCardEffect
{
    Func<CardSource, bool> CanNotSetBondCondition { get; set; }
    public void SetUpCanNotCriticalClass(Func<CardSource, bool> CanNotSetBondCondition)
    {
        this.CanNotSetBondCondition = CanNotSetBondCondition;
    }

    bool ICanNotSetBondCardEffect.CanNotSetBond(CardSource cardSource)
    {
        if (CanNotSetBondCondition(cardSource))
        {
            return true;
        }

        return false;
    }
}
