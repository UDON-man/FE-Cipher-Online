using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Jeigan_OldPowerful : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotSetBondClass canNotSetBondClass = new CanNotSetBondClass();
        canNotSetBondClass.SetUpICardEffect("戦場の教育役","", null, null, -1, false,card);
        canNotSetBondClass.SetUpCanNotCriticalClass((cardSource) => cardSource == card);
        cardEffects.Add(canNotSetBondClass);

        return cardEffects;
    }
}

