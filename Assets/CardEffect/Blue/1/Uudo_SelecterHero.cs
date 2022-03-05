using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Uudo_SelecterHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("聖魔剣 ホーリーデビルソード", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("ウード")) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Radiant Dawn";
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power * 2, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
        canNotCriticalClass.SetUpICardEffect("古の魔剣 ミストルティン", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false);
        canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == card.UnitContainingThisCharacter());
        canNotCriticalClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(canNotCriticalClass);

        StrikeUpClass strikeUpClass = new StrikeUpClass();
        strikeUpClass.SetUpICardEffect("古の魔剣 ミストルティン", new List<Cost>(), new List<Func<Hashtable, bool>>(), -1, false);
        strikeUpClass.SetUpStrikeUpClass((unit,Strike) => 2,(unit) =>unit == card.UnitContainingThisCharacter());
        strikeUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(strikeUpClass);

        return cardEffects;
    }
}