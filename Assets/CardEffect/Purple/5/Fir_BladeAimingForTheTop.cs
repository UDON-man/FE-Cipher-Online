using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fir_BladeAimingForTheTop : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("武者修行", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Sword), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
