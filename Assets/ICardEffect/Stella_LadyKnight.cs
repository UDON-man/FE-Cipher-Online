using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stella_LadyKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy1 = new PowerUpByEnemy();
        powerUpByEnemy1.SetUpPowerUpByEnemyWeapon("ラグズボウ", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.DragonStone) || enemyUnit.Weapons.Contains(Weapon.Beast), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy1);

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
