using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grego_NoEnemyArmy : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedDuringBattleAlly)
        {
            activateClass[0].SetUpICardEffect("グレゴと幼き竜", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Gregor and the Little Dragon";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Defender"))
                    {
                        if (hashtable["Defender"] is Unit)
                        {
                            Unit unit = (Unit)hashtable["Defender"];

                            if (unit != null)
                            {
                                if (unit.Character != null)
                                {
                                    if (unit.Character == card)
                                    {
                                        if (card.Owner.TrashCards.Contains(card))
                                        {
                                            return true;
                                        }
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
                card.Owner.TrashCards.Remove(card);
                card.Owner.InfinityCards.Remove(card);
                card.Owner.HandCards.Remove(card);

                yield return StartCoroutine(new ISetBondCard(card, true).SetBond());
                yield return StartCoroutine(card.Owner.bondObject.SetBond_Skill(card.Owner));

                yield return new WaitForSeconds(0.2f);
            }
        }

        return cardEffects;
    }
}