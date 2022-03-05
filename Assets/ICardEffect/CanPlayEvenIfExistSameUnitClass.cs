using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanPlayEvenIfExistSameUnitClass : ICardEffect,ICanPlayEvenIfExistSameUnit
{
    Func<CardSource, bool> CanPlayCondition { get; set; }

    public void SetUpCanPlayEvenIfExistSameUnitClass(Func<CardSource, bool> CanPlayCondition)
    {
        this.CanPlayCondition = CanPlayCondition;
    }

    public bool CanPlayEvenIfExistSameUnit(CardSource cardSource)
    {
        if (CanPlayCondition(cardSource))
        {
            return true;
        }

        return false;
    }

    
}
