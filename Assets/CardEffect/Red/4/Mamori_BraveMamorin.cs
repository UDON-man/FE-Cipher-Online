using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mamori_BraveMamorin : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("アックスボンバー", "Axe Bomber", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("ドーガ")) > 0)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("レンチン☆ソウル", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending,card);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }
}
