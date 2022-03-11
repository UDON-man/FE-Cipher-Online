using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Ignis_HeavyKnightKagami : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("重装の心得", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending, card);
        cardEffects.Add(powerUpByEnemy);

        PowerUpByEnemy powerUpByEnemy1 = new PowerUpByEnemy();
        powerUpByEnemy1.SetUpICardEffect("国境を越える盾","",null,new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        powerUpByEnemy1.SetUpPowerUpByEnemyWeapon("国境を越える盾", (enemyUnit, Power) => Power + 10, DefenseCondition, (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending, card);
        cardEffects.Add(powerUpByEnemy1);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.White)) >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        bool DefenseCondition(Unit unit)
        {
            if(unit != null)
            {
                if(unit.Character != null)
                {
                    if(unit != card.UnitContainingThisCharacter() && unit.Character.cardColors.Contains(CardColor.White) && unit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}
