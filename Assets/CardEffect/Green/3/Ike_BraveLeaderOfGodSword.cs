using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ike_BraveLeaderOfGodSword : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("神剣ラグネル", "Ragnell, the Sacred Blade",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            activateClass.SetLvS(card.UnitContainingThisCharacter(), 3);
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.UnitContainingThisCharacter() != null)
                {
                    if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine1()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("救国の勇将", "Brave General of Salvation",new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("アイク")) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, 1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            activateClass.SetLvS(card.UnitContainingThisCharacter(), 4);
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine1()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit.Character.Owner == card.Owner, true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("受け継がれた剣技", "", null, null, -1, false, card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 5);
        cardEffects.Add(powerUpClass1);

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("受け継がれた剣技", "", null, null, -1, false, card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
        strikeModifyClass.SetLvS(card.UnitContainingThisCharacter(), 5);
        cardEffects.Add(strikeModifyClass);

        return cardEffects;
    }
}