using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Yofa_JuniorInMercenaryBros : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("ボーレ") || unit.Character.UnitNames.Contains("オスカー")),
                    CanTargetCondition_ByPreSelecetedList: CanTargetCondition_ByPreSelecetedList,
                    CanEndSelectCondition: CanEndSelectCondition,
                    MaxCount: 2,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("トライアングルアタック", "Triangle Attack",new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
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
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ボーレ")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("ボーレ"))
                    {
                        return false;
                    }
                }

                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("オスカー")) > 0)
                {
                    if (addUnit.Character.UnitNames.Contains("オスカー"))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool CanEndSelectCondition(List<Unit> PreSelectedUnits)
            {
                if (PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("ボーレ") && !unit.IsTapped) == 1 && PreSelectedUnits.Count((unit) => unit.Character.UnitNames.Contains("オスカー") && !unit.IsTapped) == 1)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass _powerUpClass = new PowerModifyClass();
                _powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add((_timing) => _powerUpClass);

                CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add((_timing) => canNotBeEvadedClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
