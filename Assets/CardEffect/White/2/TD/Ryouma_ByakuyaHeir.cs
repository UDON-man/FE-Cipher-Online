using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ryouma_ByakuyaHeir : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神器 雷神刀", "Raijinto", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        StrikeModifyClass strikeUpClass = new StrikeModifyClass();
        strikeUpClass.SetUpICardEffect("諦めなどしない!","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        strikeUpClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
        cardEffects.Add(strikeUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            foreach(CardSource cardSource in card.Owner.BondCards)
            {
                if(!cardSource.IsReverse)
                {
                    if(cardSource.cardColors.Count((cardColor) => cardColor != CardColor.White && cardColor != CardColor.Colorless) > 0)
                    {
                        return false;
                    }
                }
            }

            if (card.Owner.OrbCards.Count == 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
