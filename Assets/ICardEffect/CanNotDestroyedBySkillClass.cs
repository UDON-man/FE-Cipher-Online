using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanNotDestroyedBySkillClass : ICardEffect, ICanNotDestroyedBySkill
{
    Func<Unit, bool> UnitCondition { get; set; }
    Func<ICardEffect, bool> SkillCondition { get; set; }
    public void SetUpCanNotDestroyedBySkillClass(Func<Unit, bool> UnitCondition, Func<ICardEffect, bool> SkillCondition)
    {
        this.UnitCondition = UnitCondition;
        this.SkillCondition = SkillCondition;
    }

    public bool CanNotDestroyedBySkill(Unit unit, ICardEffect skill)
    {
        if(unit != null && skill != null)
        {
            if(UnitCondition(unit) && SkillCondition(skill))
            {
                return true;
            }
        }

        return false;
    }
}
