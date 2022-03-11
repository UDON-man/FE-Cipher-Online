using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Marks_AnnyaKingSon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神器 ジークフリート", "Sacred Treasure, Siegfried",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
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

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("暗夜の騎士道", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(),false);
        cardEffects.Add(strikeModifyClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            foreach (CardSource cardSource in card.Owner.BondCards)
            {
                if (!cardSource.IsReverse)
                {
                    if (cardSource.cardColors.Count((cardColor) => cardColor != CardColor.Black && cardColor != CardColor.Colorless) > 0)
                    {
                        return false;
                    }
                }
            }

            if (card.Owner.Enemy.OrbCards.Count >= 4)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
