using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangeCriticalMagnificationClass : ICardEffect,IChangeCriticalMagnificationEffect
{
    Func<Unit, bool> ChangeCriticalMagnificationCondition { get; set; }
    Func<Unit,int,int> CriticalMagnificationFunction;
    public void SetUpChangeCriticalMagnificationClass(Func<Unit, int, int> CriticalMagnificationFunction, Func<Unit, bool> ChangeCriticalMagnificationCondition)
    {
        this.ChangeCriticalMagnificationCondition = ChangeCriticalMagnificationCondition;
        this.CriticalMagnificationFunction = CriticalMagnificationFunction;
    }

    public int GetCriticalMagnification(Unit unit,int CriticalMagnification)
    {
        if (ChangeCriticalMagnificationCondition(unit))
        {
            CriticalMagnification = this.CriticalMagnificationFunction(unit, CriticalMagnification);
        }

        return CriticalMagnification;
    }
}
