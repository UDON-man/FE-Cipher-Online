using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Uudo_SelecterHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("聖魔剣 ホーリーデビルソード", "Radiant Dawn", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("ウード")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
        canNotCriticalClass.SetUpICardEffect("古の魔剣 ミストルティン","", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false,card);
        canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == card.UnitContainingThisCharacter());
        canNotCriticalClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(canNotCriticalClass);

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("古の魔剣 ミストルティン", "",new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false,card);
        strikeModifyClass.SetUpStrikeModifyClass((unit,Strike) => 2,(unit) =>unit == card.UnitContainingThisCharacter(), false);
        strikeModifyClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(strikeModifyClass);

        return cardEffects;
    }
}