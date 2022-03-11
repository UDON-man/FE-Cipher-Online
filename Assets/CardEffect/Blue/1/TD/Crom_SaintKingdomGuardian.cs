using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crom_SaintKingdomGuardian : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("封剣 ファルシオン", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);


        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("クロム自警団", "Chrom's Shepherds", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine(hashtable));
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if(Unit.Character != null)
                            {
                                if(Unit.Character.Owner == card.Owner && Unit != card.UnitContainingThisCharacter())
                                {
                                    return true;
                                }
                            }
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
                            Unit Unit = (Unit)hashtable["Unit"];

                            if(Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner && Unit != card.UnitContainingThisCharacter())
                                {
                                    PowerModifyClass powerUpClass = new PowerModifyClass();
                                    powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == Unit, true);
                                    Unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                                    StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                                    strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == Unit, false);
                                    Unit.UntilEachTurnEndUnitEffects.Add((_timing) => strikeModifyClass);
                                }
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

