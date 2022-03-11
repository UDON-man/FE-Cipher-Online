using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Irace_HungrySorcerer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("一飯の恩", "Insatiable Appetite", new List<Cost>() { new DiscardHandCost(1, (cardSource) => !cardSource.UnitNames.Contains("イレース")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);
                yield return null;
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpICardEffect("屠竜の雷","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("屠竜の雷", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.cEntity_EffectController.GetUseCountThisTurn("一飯の恩") > 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}