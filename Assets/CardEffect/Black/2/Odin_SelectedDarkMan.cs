using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Odin_SelectedDarkMan : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("百の必殺技", "A Hundred Special Techniques",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("禁断魔書 カイザー・クヴァール", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
        strikeModifyClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(strikeModifyClass );

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.Enemy.HandCards.Count == 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}
