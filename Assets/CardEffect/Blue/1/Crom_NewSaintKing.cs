using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crom_NewSaintKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("神剣 ファルシオン", (enemyUnit, Power) => Power + 40, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        powerUpByEnemy.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("聖王の威光", "Authority of the Exalt", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("クロム")) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: card.Owner.Enemy.FieldUnit.Count,
                    CanNoSelect: true,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                card.Owner.UntilEachTurnEndEffects.Add((_timing) => new AllyPowerUp(new List<Func<Unit, bool>>() { PowerUpConditon }, PlusPower));

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