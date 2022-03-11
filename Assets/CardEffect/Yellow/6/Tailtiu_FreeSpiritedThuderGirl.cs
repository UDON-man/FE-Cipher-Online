using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Tailtiu_FreeSpiritedThuderGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCriticalAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("怒り", "Wrath", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            if(card.Owner.OrbCards.Count <= 2)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        else if (timing == EffectTiming.OnDestroyedDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("もう!やんなっちゃう", "No I... I can't take much more of this...", new List<Cost>() { new DiscardHandCost(1,(cardSource) => true)}, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

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
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}

