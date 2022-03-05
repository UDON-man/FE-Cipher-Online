using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangePlaceDestroyedUnitClass : ICardEffect, IChangePlaceDestroyedUnitEffect
{
    Func<Unit, bool> DestroyedUnitCondition { get; set; }
    Func<Unit, DestroyMode> PlaceDestroyedUnit { get; set; }
    public void SetUpChangePlaceDestroyedUnitClass(Func<Unit, DestroyMode> PlaceDestroyedUnit, Func<Unit, bool> DestroyedUnitCondition)
    {
        this.DestroyedUnitCondition = DestroyedUnitCondition;
        this.PlaceDestroyedUnit = PlaceDestroyedUnit;
    }

    public DestroyMode GetDestroyMode(Unit Defender)
    {
        DestroyMode destroyMode = DestroyMode.Trash;

        if(PlaceDestroyedUnit != null)
        {
            destroyMode = PlaceDestroyedUnit(Defender);
        }

        return destroyMode;
    }
}
