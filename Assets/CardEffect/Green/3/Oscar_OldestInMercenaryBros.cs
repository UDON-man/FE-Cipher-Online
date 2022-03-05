using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class Oscar_OldestInMercenaryBros : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("頼れる長兄", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ボーレ")) > 0 && card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ヨファ")) > 0)
            {
                return true;
            }

            return false;
        }

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("ボーレ") || unit.Character.UnitNames.Contains("ヨファ")),
                    CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    CanEndSelectCondition: CanEndSelectCondition,
                    MaxCount: 2,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Tap);

            activateClass[0].SetUpICardEffect("トライアングルアタック", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Triangle Attack";
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

            bool CanTargetCondition_ByPreSelecetedList(List<Unit> PreSelectedUnits, Unit addUnit)
            {
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ボーレ")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("ボーレ"))
                    {
                        return false;
                    }
                }

                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ヨファ")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("ヨファ"))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool CanEndSelectCondition(List<Unit> PreSelectedUnits)
            {
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ボーレ") && !unit.IsTapped) == 1 && PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ヨファ") && !unit.IsTapped) == 1)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass _powerUpClass = new PowerUpClass();
                _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add(_powerUpClass);

                CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
                canNotAttackClass.SetUpCanNotAttackClass((AttackingUnit) => AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit.Character.Owner == this.card.Owner);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add(canNotAttackClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
