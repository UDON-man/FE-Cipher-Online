using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanaki_YoungEmperor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("オルティナの末裔", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Horse) || enemyUnit.Weapons.Contains(Weapon.Wing) || enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
