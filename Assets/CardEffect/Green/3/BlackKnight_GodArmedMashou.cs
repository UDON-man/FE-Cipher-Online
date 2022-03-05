using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlackKnight_GodArmedMashou : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("神剣エタルド", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Alondite, the Sacred Blade";
            }

            IEnumerator ActivateCoroutine()
            {
                StrikeUpClass strikeUpClass = new StrikeUpClass();
                strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add(strikeUpClass);

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add(rangeUpClass);

                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add(powerUpClass);

                yield return null;
            }
        }

        CanNotDestroyedByBattleClass canNotDestroyedByBattleClass = new CanNotDestroyedByBattleClass();
        canNotDestroyedByBattleClass.SetUpICardEffect("女神の加護",new List<Cost>(),new List<Func<Hashtable, bool>>(),-1,false);
        canNotDestroyedByBattleClass.SetUpCanNotDestroyedByBattleClass((AttackingUnit) => AttackingUnit.Character.PlayCost <= 2,(DefendingUnit) => DefendingUnit == card.UnitContainingThisCharacter());
        cardEffects.Add(canNotDestroyedByBattleClass);

        return cardEffects;
    }
}
