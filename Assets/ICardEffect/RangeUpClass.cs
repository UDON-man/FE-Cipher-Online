using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class RangeUpClass : ICardEffect, IChangeRangeEffect
{
    public void SetUpRangeUpClass(Func<Unit, List<int>, List<int>> ChangeRange, Func<Unit, bool> CanRangeUpCondition)
    {
        this.ChangeRange = ChangeRange;
        this.CanRangeUpCondition = CanRangeUpCondition;
    }

    Func<Unit, List<int>, List<int>> ChangeRange { get; set; }
    Func<Unit, bool> CanRangeUpCondition;

    public List<int> GetRange(List<int> Range, Unit unit)
    {
        if(CanRangeUpCondition(unit))
        {
            Range = ChangeRange(unit,Range);
        }

        return Range;
    }
}