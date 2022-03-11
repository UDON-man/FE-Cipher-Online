using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Jerome_FlyingBlackWing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("剣殺し", (enemyUnit,Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Sword), PowerUpByEnemy.Mode.Both,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ショートアクス", "Short Axe",new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}

