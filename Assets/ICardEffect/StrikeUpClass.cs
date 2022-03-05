using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StrikeUpClass : ICardEffect, IChangeDamageCardEffect
{
    public void SetUpStrikeUpClass(Func<Unit, int, int> ChangeStrike, Func<Unit, bool> CanStrikeUpCondition)
    {
        this.ChangeStrike = ChangeStrike;
        this.CanStrikeUpCondition = CanStrikeUpCondition;
    }

    Func<Unit, int, int> ChangeStrike { get; set; }
    Func<Unit, bool> CanStrikeUpCondition;

    public int GetDamage(int Damage, Unit unit)
    {
        if (CanStrikeUpCondition(unit))
        {
            Damage = ChangeStrike(unit, Damage);
        }

        return Damage;
    }
}