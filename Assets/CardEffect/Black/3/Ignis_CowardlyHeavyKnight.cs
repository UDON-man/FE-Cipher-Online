using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ignis_CowardlyHeavyKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("重装の心得", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending,card);
        cardEffects.Add(powerUpByEnemy);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("誰かを守る強さ","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        powerUpClass.SetUpPowerUpClass((unit,Power) => Power + 20,(unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if(GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
            {
                if(card.Owner.GetBackUnits().Count((unit) => unit != card.UnitContainingThisCharacter()) >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }

}
