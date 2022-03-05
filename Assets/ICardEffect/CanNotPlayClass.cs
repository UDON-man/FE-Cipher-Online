using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotPlayClass : ICardEffect,ICanNotPlayCardEffect
{
    Func<CardSource, bool> CanNotPlayCondition { get; set; }

    public void SetUpCanNotPlayClass(Func<CardSource, bool> CanNotPlayCondition)
    {
        this.CanNotPlayCondition = CanNotPlayCondition;
    }

    public bool CanNotPlay(CardSource cardSource)
    {
        if (CanNotPlayCondition(cardSource))
        {
            return true;
        }

        return false;
    }

    
}
