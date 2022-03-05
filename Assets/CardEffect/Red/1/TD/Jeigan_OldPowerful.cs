using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Jeigan_OldPowerful : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotSetBondClass canNotSetBondClass = new CanNotSetBondClass();
        canNotSetBondClass.SetUpICardEffect("戦場の教育役", null, null, -1, false);
        canNotSetBondClass.SetUpCanNotCriticalClass((cardSource) => cardSource == this.card);
        cardEffects.Add(canNotSetBondClass);

        return cardEffects;
    }
}

