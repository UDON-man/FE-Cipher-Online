using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Elfie_GouwanFunsai : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy1 = new PowerUpByEnemy();
        powerUpByEnemy1.SetUpPowerUpByEnemyWeapon("重装の心得", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending, card);
        cardEffects.Add(powerUpByEnemy1);

        PowerUpByEnemy powerUpByEnemy2 = new PowerUpByEnemy();
        powerUpByEnemy2.SetUpICardEffect("重装の覚悟", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => card.Owner.OrbCards.Count < card.Owner.Enemy.OrbCards.Count }, -1, false, card);
        powerUpByEnemy2.SetUpPowerUpByEnemyWeapon("重装の覚悟", (enemyUnit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending, card);
        cardEffects.Add(powerUpByEnemy2);

        return cardEffects;
    }
}
