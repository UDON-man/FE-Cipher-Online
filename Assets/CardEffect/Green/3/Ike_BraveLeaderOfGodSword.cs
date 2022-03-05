using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ike_BraveLeaderOfGodSword : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("神剣ラグネル", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            activateClass[0].SetLvS(card.UnitContainingThisCharacter(), 3);
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Ragnell, the Sacred Blade";
            }

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
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(rangeUpClass);

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            activateClass[1].SetUpICardEffect("救国の勇将", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("アイク")) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, 1, true);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine1());
            activateClass[1].SetLvS(card.UnitContainingThisCharacter(), 4);
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Brave General of Salvation";
            }

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
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
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit.Character.Owner == card.Owner);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        PowerUpClass powerUpClass1 = new PowerUpClass();
        powerUpClass1.SetUpICardEffect("受け継がれた剣技",null,null,-1,false);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass1.SetLvS(card.UnitContainingThisCharacter(), 5);
        cardEffects.Add(powerUpClass1);

        StrikeUpClass strikeUpClass = new StrikeUpClass();
        strikeUpClass.SetUpICardEffect("受け継がれた剣技", null, null, -1, false);
        strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter());
        strikeUpClass.SetLvS(card.UnitContainingThisCharacter(), 5);
        cardEffects.Add(strikeUpClass);

        return cardEffects;
    }
}