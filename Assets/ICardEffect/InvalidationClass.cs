using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InvalidationClass : ICardEffect,IInvalidationCardEffect
{
    public void SetUpInvalidationClass(Func<ICardEffect, bool> InvalidateCondition)
    {
        this.InvalidateCondition = InvalidateCondition;
    }

    Func<ICardEffect,bool> InvalidateCondition { get; set; }
    public bool IsInvalidate(ICardEffect cardEffect)
    {
        if(InvalidateCondition(cardEffect))
        {
            return true;
        }

        return false;
    }
}
