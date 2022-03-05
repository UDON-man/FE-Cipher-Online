using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Nowaru_TwoMind : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("母に習った呪い", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サーリャ")) == 0)
            {
                return false;
            }

            return true;
        }

        bool PowerUpCondition(Unit unit)
        {
            if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
            {
                if (unit == card.UnitContainingThisCharacter())
                {
                    if(card.Owner.Enemy.HandCards.Count < card.Owner.HandCards.Count)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}

