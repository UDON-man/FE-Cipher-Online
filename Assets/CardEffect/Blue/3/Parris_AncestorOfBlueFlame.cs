using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Parris_AncestorOfBlueFlame : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("比類なき戦技", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Sword)|| enemyUnit.Weapons.Contains(Weapon.Lance)|| enemyUnit.Weapons.Contains(Weapon.Axe), PowerUpByEnemy.Mode.Both);
        cardEffects.Add(powerUpByEnemy);

        ChangeCardColorsClass changeCardColorsClass = new ChangeCardColorsClass();
        changeCardColorsClass.SetUpICardEffect("蒼炎を継ぐ者", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        changeCardColorsClass.SetUpCardColorChangeClass((cardSource, cardColors) => { cardColors.Add(CardColor.Green); return cardColors; }, (cardSource) => cardSource == card);
        cardEffects.Add(changeCardColorsClass);

        bool CanUseCondition2(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

