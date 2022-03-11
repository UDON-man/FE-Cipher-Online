using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Lufure_SecretTactics : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("戦知識", "Guerrilla Warfare", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit == card.UnitContainingThisCharacter())
                                {
                                    if (card.Owner.OrbCount < card.Owner.Enemy.OrbCount)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }
            
            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.OrbCount < card.Owner.Enemy.OrbCount)
                {
                    yield return StartCoroutine(new IAddOrbFromLibrary(card.Owner, 1).AddOrb());
                }
            }
        }

        return cardEffects;
    }
}
