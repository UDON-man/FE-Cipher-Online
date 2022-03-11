using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Olug_SilentFang : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("野生のセンス", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Character.Owner.GetBackUnits().Contains(enemyUnit), PowerUpByEnemy.Mode.Both,card);
        cardEffects.Add(powerUpByEnemy);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("化身","", new List<Cost>(), new List<System.Func<Hashtable, bool>>(), -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass);

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("化身","",new List<Cost>(),new List<System.Func<Hashtable, bool>>(),-1,false,card);
        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
        canAttackTargetUnitRegardlessRangeClass.SetLvS(card.UnitContainingThisCharacter(),2);
        cardEffects.Add(canAttackTargetUnitRegardlessRangeClass);

        return cardEffects;
    }
}

