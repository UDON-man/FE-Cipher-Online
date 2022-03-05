using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Oguma_BattleGodOfTaris : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && !unit.IsTapped,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Tap);

            activateClass[0].SetUpICardEffect("戦場の息吹", new List<Cost>() { selectAllyCost }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Battlefield Breath";
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());

                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                yield return null;
            }
        }

        else if(timing == EffectTiming.OnAttackAnyone)
        {
            activateClass[1].SetUpICardEffect("闘神の一撃", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Wargod's Blow";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if(this.card.UnitContainingThisCharacter().Power >= 100)
                        {
                            return true;
                        }
                        
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                StrikeUpClass strikeUpClass = new StrikeUpClass();
                strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter());

                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(strikeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}