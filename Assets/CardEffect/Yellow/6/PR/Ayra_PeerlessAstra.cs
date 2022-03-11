using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Ayra_PeerlessAstra : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("連星刃", "Astrum", new List<Cost>() { new ReverseCost(3, (cardSource) => true) }, new List<Func<Hashtable, bool>>() , -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));

                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        return cardEffects;
    }
}




