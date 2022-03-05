using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SupportPowerUpClass : ICardEffect, IChangeSupportPowerCardEffect
{
    public void SetUpSupportPowerUpClass(Func<CardSource, int, int> ChangeSupportPower, Func<CardSource, bool> ChangeSupportPowerCondition)
    {
        this.ChangeSupportPower = ChangeSupportPower;
        this.ChangeSupportPowerCondition = ChangeSupportPowerCondition;
    }

    Func<CardSource, int, int> ChangeSupportPower { get; set; }
    Func<CardSource, bool> ChangeSupportPowerCondition;


    public int GetSupportPower(int SupportPower, CardSource cardSource)
    {
        if (ChangeSupportPowerCondition(cardSource))
        {
            SupportPower = ChangeSupportPower(cardSource, SupportPower);
        }

        return SupportPower;
    }
}