using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crom_SaintKingdomGuardian : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("封剣 ファルシオン", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        cardEffects.Add(powerUpByEnemy);


        if (timing == EffectTiming.OnCCAnyone)
        {
            activateClass[0].SetUpICardEffect("クロム自警団", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Chrom's Shepherds";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit CCUnit = (Unit)hashtable["Unit"];

                            return CCUnit.Character.Owner == this.card.Owner && CCUnit != this.card.UnitContainingThisCharacter();
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit CCUnit = (Unit)hashtable["Unit"];

                            if(CCUnit.Character.Owner == this.card.Owner && CCUnit != this.card.UnitContainingThisCharacter())
                            {
                                PowerUpClass powerUpClass = new PowerUpClass();
                                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == CCUnit);
                                CCUnit.UntilEachTurnEndUnitEffects.Add(powerUpClass);

                                StrikeUpClass strikeUpClass = new StrikeUpClass();
                                strikeUpClass.SetUpStrikeUpClass((unit, Strike) => 2, (unit) => unit == CCUnit);
                                CCUnit.UntilEachTurnEndUnitEffects.Add(strikeUpClass);
                            }
                        }
                    }
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}

