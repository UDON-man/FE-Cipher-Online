using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangePlayCostClass : ICardEffect, IChangePlayCost
{
    public void SetUpChangeCCCostClass(Func<CardSource, int, int> ChangePlayCost, Func<CardSource, bool> CanChangePlayCostCondition, bool _isUpDown)
    {
        this.ChangePlayCost = ChangePlayCost;
        this.CanChangePlayCostCondition = CanChangePlayCostCondition;
        this._isUpDown = _isUpDown;
    }

    Func<CardSource, int, int> ChangePlayCost { get; set; }
    Func<CardSource, bool> CanChangePlayCostCondition { get; set; }
    bool _isUpDown { get; set; }

    public int GetPlayCost(int PlayCost, CardSource cardSource)
    {
        if (CanChangePlayCostCondition(cardSource))
        {
            PlayCost = ChangePlayCost(cardSource, PlayCost);
        }

        return PlayCost;
    }

    public bool isUpDown()
    {
        return this._isUpDown;
    }
}