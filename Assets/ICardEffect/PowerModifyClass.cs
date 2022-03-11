using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PowerModifyClass : ICardEffect, IPowerModifyCardEffect
{
    public void SetUpPowerUpClass(Func<Unit, int, int> ChangePower, Func<Unit, bool> CanPowerModifyCondition, bool _isUpDown)
    {
        this.ChangePower = ChangePower;
        this.CanPowerModifyCondition = CanPowerModifyCondition;
        this._isUpDown = _isUpDown;
    }

    Func<Unit, int, int> ChangePower { get; set; }
    Func<Unit, bool> CanPowerModifyCondition;
    bool _isUpDown;

    public int GetPower(int Power, Unit unit)
    {
        if (CanPowerModifyCondition(unit))
        {
            Power = ChangePower(unit, Power);
        }

        return Power;
    }

    public bool isUpDown()
    {
        return _isUpDown;
    }
}