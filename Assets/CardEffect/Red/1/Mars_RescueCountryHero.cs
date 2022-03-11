using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Mars_RescueCountryHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("ファルシオン", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("英雄の凱歌", "Hero's Paean",new List<Cost>() { new ReverseCost(3,(cardSource) => true), new DiscardHandCost(1,(cardSource) => cardSource.UnitNames.Contains("マルス")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                card.Owner.UntilOpponentTurnEndEffects.Add((_timing) => new AllyPowerUp(new List<Func<Unit, bool>>() { PowerUpConditon }, PlusPower));

                bool PowerUpConditon(Unit unit)
                {
                    if (unit.Character != null)
                    {
                        if (card.Owner.FieldUnit.Contains(unit))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                int PlusPower(Unit unit, int Power)
                {
                    return Power + 30;
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}

public class AllyPowerUp : ICardEffect, IPowerModifyCardEffect
{
    public AllyPowerUp(List<Func<Unit, bool>> PowerUpConditons,Func<Unit,int,int> PlusPower)
    {
        this.PowerUpConditons = PowerUpConditons;
        this.PlusPower = PlusPower;
    }

    List<Func<Unit, bool>> PowerUpConditons { get; set; } = new List<Func<Unit, bool>>();
    Func<Unit, int, int> PlusPower { get; set; }

    public int GetPower(int Power, Unit unit)
    {
        //対象のユニットが条件全てを満たすなら
        if(PowerUpConditons.Count == PowerUpConditons.Count((PowerUpConditons) => PowerUpConditons(unit)))
        {
            Power = PlusPower(unit, Power);
        }

        return Power;
    }

    public bool isUpDown()
    {
        return true;
    }
}
