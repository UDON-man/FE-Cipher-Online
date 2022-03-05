using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ChangeCCCostClass : ICardEffect, IChangeCCCostEffect
{
    public void SetUpChangeCCCostClass(Func<CardSource, Unit, int, int> ChangeCCCost, Func<CardSource, bool> CanChangeCCCostCondition)
    {
        this.ChangeCCCost = ChangeCCCost;
        this.CanChangeCCCostCondition = CanChangeCCCostCondition;
    }

    Func<CardSource, Unit, int, int> ChangeCCCost { get; set; }
    Func<CardSource, bool> CanChangeCCCostCondition { get; set; }

    public int GetCCCost(int CCCost, Unit targetUnit, CardSource cardSource)
    {
        if(CanChangeCCCostCondition(cardSource))
        {
            CCCost = ChangeCCCost(cardSource, targetUnit, CCCost);
        }

        return CCCost;
    }
}