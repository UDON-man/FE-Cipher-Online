using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class UnitNamesChangeClass : ICardEffect, IChangeUnitNameEffect
{
    public void SetUpUnitNamesChangeClass(Func<CardSource, List<string>, List<string>> ChangeUnitNames, Func<CardSource, bool> CanUnitNamesChangeCondition)
    {
        this.ChangeUnitNames = ChangeUnitNames;
        this.CanUnitNamesChangeCondition = CanUnitNamesChangeCondition;
    }

    Func<CardSource, List<string>, List<string>> ChangeUnitNames { get; set; }
    Func<CardSource, bool> CanUnitNamesChangeCondition;

    public List<string> GetUnitNames(List<string> UnitNames, CardSource cardSource)
    {
        if (CanUnitNamesChangeCondition(cardSource))
        {
            UnitNames = ChangeUnitNames(cardSource, UnitNames);
        }

        return UnitNames;
    }
}