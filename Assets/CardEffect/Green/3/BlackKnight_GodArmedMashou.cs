using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlackKnight_GodArmedMashou : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神剣エタルド", "Alondite, the Sacred Blade", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => strikeModifyClass);

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => rangeUpClass);

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        CanNotDestroyedByBattleClass canNotDestroyedByBattleClass = new CanNotDestroyedByBattleClass();
        canNotDestroyedByBattleClass.SetUpICardEffect("女神の加護","",new List<Cost>(),new List<Func<Hashtable, bool>>(),-1,false,card);
        canNotDestroyedByBattleClass.SetUpCanNotDestroyedByBattleClass((AttackingUnit) => AttackingUnit.Character.PlayCost <= 2,(DefendingUnit) => DefendingUnit == card.UnitContainingThisCharacter());
        cardEffects.Add(canNotDestroyedByBattleClass);

        return cardEffects;
    }
}
