using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangePlayCostClass : ICardEffect, IChangePlayCost
{
    public void SetUpChangeCCCostClass(Func<CardSource, int, int> ChangePlayCost, Func<CardSource, bool> CanChangePlayCostCondition)
    {
        this.ChangePlayCost = ChangePlayCost;
        this.CanChangePlayCostCondition = CanChangePlayCostCondition;
    }

    Func<CardSource, int, int> ChangePlayCost { get; set; }
    Func<CardSource, bool> CanChangePlayCostCondition { get; set; }

    public int GetPlayCost(int PlayCost, CardSource cardSource)
    {
        if (CanChangePlayCostCondition(cardSource))
        {
            PlayCost = ChangePlayCost(cardSource, PlayCost);
        }

        return PlayCost;
    }
}