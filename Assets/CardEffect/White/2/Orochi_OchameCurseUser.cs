using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orochi_OchameCurseUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("鼠神・子", "Rat Spirit", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                yield return StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}