using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Takumi_GodArcher : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("巧者の和弓", "True Craftsmanship", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
                canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => canAttackTargetUnitRegardlessRangeClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}

