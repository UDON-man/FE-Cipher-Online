using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Olug_SilentFang : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("野生のセンス", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Character.Owner.GetBackUnits().Contains(enemyUnit), PowerUpByEnemy.Mode.Both);
        cardEffects.Add(powerUpByEnemy);

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("化身", new List<Cost>(), new List<System.Func<Hashtable, bool>>(), -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass);

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("化身",new List<Cost>(),new List<System.Func<Hashtable, bool>>(),-1,false);
        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
        canAttackTargetUnitRegardlessRangeClass.SetLvS(card.UnitContainingThisCharacter(),2);
        cardEffects.Add(canAttackTargetUnitRegardlessRangeClass);

        return cardEffects;
    }
}

