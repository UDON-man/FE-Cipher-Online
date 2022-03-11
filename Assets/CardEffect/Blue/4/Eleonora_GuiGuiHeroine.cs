using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Eleonora_GuiGuiHeroine : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("ときめき片想い","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(rangeUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ヴィオール")) > 0)
            {
                return true;
            }

            return false;
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);


        return cardEffects;
    }
}
