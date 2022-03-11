using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Ardan_StrudyStrongSlow : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("固い!!", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending, card);
        cardEffects.Add(powerUpByEnemy);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("強い!!", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }
            }

            return false;
        }

        CanNotMoveClass canNotMoveClass = new CanNotMoveClass();
        canNotMoveClass.SetUpICardEffect("おそい!!", "", null, null, -1, false, card);
        canNotMoveClass.SetUpCanNotMoveClass(CanPowerUpCondition);
        cardEffects.Add(canNotMoveClass);

        return cardEffects;
    }
}
