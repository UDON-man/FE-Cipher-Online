using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Eleonora_PreciousActress : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("運命の一射", "Burning Fate", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("弓弦エレオノーラ") || cardSource.UnitNames.Contains("ヴィオール")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
                    canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == unit && unit.Weapons.Contains(Weapon.Bow), (DefendingUnit) => true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => canAttackTargetUnitRegardlessRangeClass);

                    CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                    canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == unit && unit.Weapons.Contains(Weapon.Bow), (DefendingUnit) => DefendingUnit != DefendingUnit.Character.Owner.Lord);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);
                }

                yield return null;
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);


        return cardEffects;
    }
}
