using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Uudo_LegendHeir : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("蒼炎剣 ブルーフレイムソード", "Sacred Stones", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
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
        strikeModifyClass.SetUpICardEffect("受け継がれし聖痕","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
        cardEffects.Add(strikeModifyClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Contains(card.UnitContainingThisCharacter()))
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() || GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                    {
                        if (card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("リズ"))>0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}
