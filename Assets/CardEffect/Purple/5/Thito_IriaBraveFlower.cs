using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Thito_IriaBraveFlower : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("イリアの三天馬", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() || GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                {
                    if (card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("ユーノ") || cardSource.UnitNames.Contains("シャニー")) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("ユーノ") || unit.Character.UnitNames.Contains("シャニー")),
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
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ユーノ")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("ユーノ"))
                    {
                        return false;
                    }
                }

                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("シャニー")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("シャニー"))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool CanEndSelectCondition(List<Unit> PreSelectedUnits)
            {
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ユーノ") && !unit.IsTapped) == 1 && PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("シャニー") && !unit.IsTapped) == 1)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit targetUnit = card.UnitContainingThisCharacter();

                PowerModifyClass _powerUpClass = new PowerModifyClass();
                _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == targetUnit, true);
                targetUnit.UntilEndBattleEffects.Add((_timing) => _powerUpClass);

                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                targetUnit.UntilEndBattleEffects.Add((_timing) => canNotBeEvadedClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
