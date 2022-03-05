using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crom_NewSaintKing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("神剣 ファルシオン", (enemyUnit, Power) => Power + 40, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking);
        powerUpByEnemy.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("聖王の威光", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("クロム")) }, null, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Authority of the Exalt";
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner != this.card.Owner,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: card.Owner.Enemy.FieldUnit.Count,
                CanNoSelect: true,
                CanEndNotMax: true,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                card.Owner.UntilTurnEndEffects.Add(new AllyPowerUp(new List<Func<Unit, bool>>() { PowerUpConditon }, PlusPower));

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