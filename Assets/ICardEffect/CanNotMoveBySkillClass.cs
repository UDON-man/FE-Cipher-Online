using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CanNotMoveBySkillClass : ICardEffect, ICanNotMoveBySkillEffect
{
    Func<Unit, bool> CanNotMoveCondition { get; set; }
    public void SetUpCanNotMoveBySkillClass(Func<Unit, bool> CanNotMoveCondition)
    {
        this.CanNotMoveCondition = CanNotMoveCondition;
    }

    public bool CanNotMoveBySkill(Unit unit)
    {
        if (CanNotMoveCondition(unit))
        {
            return true;
        }

        return false;
    }
}