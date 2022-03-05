using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ChangeSkillActivateCountClass : ICardEffect, IChangeSkillActivateCountEffect
{
    public void SetUpChangeSkillActivateCountClass(Func<ICardEffect, bool> SkillCondition, Func<ICardEffect, int, int> ChangeSkillActivateCount)
    {
        this.SkillCondition = SkillCondition;
        this.ChangeSkillActivateCount = ChangeSkillActivateCount;
    }

    Func<ICardEffect, bool> SkillCondition { get; set; }
    Func<ICardEffect,int,int> ChangeSkillActivateCount { get; set; }

    public int GetActivateCount(ICardEffect cardEffect, int ActiveCount)
    {
        if (SkillCondition(cardEffect))
        {
            ActiveCount = ChangeSkillActivateCount(cardEffect, ActiveCount);
        }

        return ActiveCount;
    }
}
