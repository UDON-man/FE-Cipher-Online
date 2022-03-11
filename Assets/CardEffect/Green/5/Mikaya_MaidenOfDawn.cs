using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mikaya_MaidenOfDawn : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("セイニー", (enemyUnit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Armor) || enemyUnit.Weapons.Contains(Weapon.Horse), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
