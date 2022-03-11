using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ike_ClimiaKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("将軍の参陣", "General Enters the Fray", new List<Cost>() ,new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit CCUnit = (Unit)hashtable["Unit"];

                            return CCUnit.Character == card;
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
