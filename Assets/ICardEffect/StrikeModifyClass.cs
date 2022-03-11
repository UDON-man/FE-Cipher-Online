using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StrikeModifyClass : ICardEffect, IStrikeModifyCardEffect
{
    public void SetUpStrikeModifyClass(Func<Unit, int, int> ChangeStrike, Func<Unit, bool> CanStrikeModifyCondition,bool _isUpDown)
    {
        this.ChangeStrike = ChangeStrike;
        this.CanStrikeModifyCondition = CanStrikeModifyCondition;
        this._isUpDown = _isUpDown;
    }

    Func<Unit, int, int> ChangeStrike { get; set; }
    Func<Unit, bool> CanStrikeModifyCondition;
    bool _isUpDown;

    public int GetDamage(int Damage, Unit unit)
    {
        if (CanStrikeModifyCondition(unit))
        {
            Damage = ChangeStrike(unit, Damage);
        }

        return Damage;
    }


    public bool isUpDown()
    {
        return _isUpDown;
    }
}