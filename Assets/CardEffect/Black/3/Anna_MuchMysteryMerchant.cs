using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Anna_MuchMysteryMerchant : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        SuccessSupportClass successSupportClass = new SuccessSupportClass();
        successSupportClass.SetUpSuccessSupportClass((cardSource) => cardSource.UnitNames.Contains("アンナ"), (unit) => unit == card.UnitContainingThisCharacter());
        successSupportClass.SetUpICardEffect("100人のアンナ","", null, null, -1, false,card);
        cardEffects.Add(successSupportClass);

        CanPlayEvenIfExistSameUnitClass canPlayEvenIfExistSameUnitClass = new CanPlayEvenIfExistSameUnitClass();
        canPlayEvenIfExistSameUnitClass.SetUpCanPlayEvenIfExistSameUnitClass((cardSource) => cardSource == card);
        canPlayEvenIfExistSameUnitClass.SetUpICardEffect("100人のアンナ","", null, null, -1, false,card);
        cardEffects.Add(canPlayEvenIfExistSameUnitClass);

        return cardEffects;
    }
}