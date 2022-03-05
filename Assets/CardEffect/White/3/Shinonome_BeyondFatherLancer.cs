using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Shinonome_BeyondFatherLancer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("槍の名手", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Sword) , PowerUpByEnemy.Mode.Both);
        cardEffects.Add(powerUpByEnemy);

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("次世代の絆", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.cardColors.Contains(CardColor.Black)) > 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}

