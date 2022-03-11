using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Takumi_PossessedDemonPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Destroy);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("スカディ", "Skadi", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>(), 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
                    canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => true);
                    card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canAttackTargetUnitRegardlessRangeClass);

                    CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass1 = new CanAttackTargetUnitRegardlessRangeClass();
                    canAttackTargetUnitRegardlessRangeClass1.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == unit, (DefendingUnit) => true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => canAttackTargetUnitRegardlessRangeClass1);

                    yield return null;
                }
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking, card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}