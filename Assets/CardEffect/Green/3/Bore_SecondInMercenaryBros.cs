using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Bore_SecondInMercenaryBros : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("戦士の心得","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("オスカー") || unit.Character.UnitNames.Contains("ヨファ")),
                    CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    CanEndSelectCondition: CanEndSelectCondition,
                    MaxCount: 2,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("トライアングルアタック", "Triangle Attack", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
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

            bool CanTargetCondition_ByPreSelecetedList(List<Unit> PreSelectedUnits, Unit addUnit)
            {
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("オスカー")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("オスカー"))
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
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("オスカー") && !unit.IsTapped) == 1 && PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ヨファ") && !unit.IsTapped) == 1)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit targetUnit = card.UnitContainingThisCharacter();

                PowerModifyClass _powerUpClass = new PowerModifyClass();
                _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 50, (unit) => unit == targetUnit, true);
                targetUnit.UntilEndBattleEffects.Add((_timing) => _powerUpClass);

                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == targetUnit, false);
                targetUnit.UntilEndBattleEffects.Add((_timing) => strikeModifyClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
