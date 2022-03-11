using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PowerUpByEnemy : ICardEffect, IPowerModifyCardEffect
{
    public enum Mode
    {
        Attacking,
        Defending,
        Both,
    }

    Func<Unit,int,int> PlusPower { get; set; }
    Func<Unit, bool> CanTargetCondition { get; set; }
    Func<Unit, bool> EnemyUnitCondition { get; set; }
    Mode mode { get; set; }

    public void SetUpPowerUpByEnemyWeapon(string EffectName, Func<Unit,int ,int> PlusPower,Func<Unit,bool> CanTargetCondition, Func<Unit, bool> EnemyUnitCondition, Mode mode,CardSource card)
    {
        this.EffectName = EffectName;
        this.PlusPower = PlusPower;
        this.CanTargetCondition = CanTargetCondition;
        this.EnemyUnitCondition = EnemyUnitCondition;
        this.mode = mode;
        this._card = card;
    }

    public int GetPower(int Power, Unit unit)
    {
        if (unit.Character != null)
        {
            if(CanTargetCondition(unit))
            {
                if(GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if(EnemyUnit() != null)
                    {
                        if(EnemyUnitCondition(EnemyUnit()))
                        {
                            Power = PlusPower(EnemyUnit(),Power);
                        }
                    }

                    Unit EnemyUnit()
                    {
                        switch (mode)
                        {
                            case Mode.Attacking:

                                if (unit == GManager.instance.turnStateMachine.AttackingUnit)
                                {
                                    return GManager.instance.turnStateMachine.DefendingUnit;
                                }

                                break;
                                

                            case Mode.Defending:

                                if (unit == GManager.instance.turnStateMachine.DefendingUnit)
                                {
                                    return GManager.instance.turnStateMachine.AttackingUnit;
                                }

                                break;

                            case Mode.Both:

                                if(unit == GManager.instance.turnStateMachine.AttackingUnit)
                                {
                                    return GManager.instance.turnStateMachine.DefendingUnit;
                                }

                                else if (unit == GManager.instance.turnStateMachine.DefendingUnit)
                                {
                                    return GManager.instance.turnStateMachine.AttackingUnit;
                                }

                                break;
                        }

                        return null;
                    }
                }
                
            }
        }

        return Power;
    }

    public bool isUpDown()
    {
        return true;
    }
}