using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PowerUpClass : ICardEffect, IChangePowerCardEffect
{
    public void SetUpPowerUpClass(Func<Unit,int,int> ChangePower, Func<Unit, bool> CanPowerUpCondition)
    {
        this.ChangePower = ChangePower;
        this.CanPowerUpCondition = CanPowerUpCondition;
    }

    Func<Unit,int, int> ChangePower { get; set; }
    Func<Unit, bool> CanPowerUpCondition;

    public int GetPower(int Power, Unit unit)
    {
        if (CanPowerUpCondition(unit))
        {
            Power = ChangePower(unit,Power);
        }

        return Power;
    }
}