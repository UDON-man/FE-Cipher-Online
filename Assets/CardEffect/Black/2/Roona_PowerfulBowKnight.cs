using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Roona_PowerfulBowKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ショートボウ", "Short Bow",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("飛行特効", (enemyUnit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Wing), PowerUpByEnemy.Mode.Attacking, card);
        cardEffects.Add(powerUpByEnemy);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("負けん気", "",new List<Cost>(), new List<System.Func<Hashtable, bool>>() , -1, false, card);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        int ChangePower(Unit unit, int Power)
        {
            int count = 0;

            foreach(Unit _unit in card.Owner.FieldUnit)
            {
                if(_unit != card.UnitContainingThisCharacter())
                {
                    if(_unit.Power >= 100)
                    {
                        count++;
                    }
                }
            }

            if(count > 0)
            {
                Power += 30;
            }

            return Power;
        }

        return cardEffects;
    }
}