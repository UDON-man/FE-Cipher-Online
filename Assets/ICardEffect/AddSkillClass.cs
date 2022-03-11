using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AddSkillClass : ICardEffect,IAddSkillEffect
{
    public void SetUpAddSkillClass(Func<CardSource, bool> CardSourceCondition, Func<CardSource, List<ICardEffect>, EffectTiming, List<ICardEffect>> GetEffects)
    {
        this.CardSourceCondition = CardSourceCondition;
        this.GetEffects = GetEffects;
    }
    Func<CardSource, bool> CardSourceCondition { get; set; }
    Func<CardSource, List<ICardEffect>, EffectTiming, List<ICardEffect>> GetEffects { get; set; }

    public List<ICardEffect> GetCardEffect(CardSource card, List<ICardEffect> GetCardEffect,EffectTiming timing)
    {
        if(CardSourceCondition(card))
        {
            GetCardEffect = GetEffects(card,GetCardEffect,timing);
        }

        return GetCardEffect;
    }
}
