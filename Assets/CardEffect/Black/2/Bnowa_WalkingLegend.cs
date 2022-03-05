using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bnowa_WalkingLegend : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("重装の心得", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending);
        cardEffects.Add(powerUpByEnemy);

        if (timing == EffectTiming.OnAttackedAlly)
        {
            activateClass[0].SetUpICardEffect("鉄壁の盾", new List<Cost>() { new ReverseCost(1,(cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Impregnable Wall";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit != this.card.UnitContainingThisCharacter())
                                {
                                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }

                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                #region 旧防御ユニットのエフェクトを削除
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.OffAttackerDefenderEffect();
                GManager.instance.OffTargetArrow();
                #endregion
                
                //防御ユニットを更新
                GManager.instance.turnStateMachine.DefendingUnit = this.card.UnitContainingThisCharacter();

                #region 新防御ユニットのエフェクトを表示
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.SetDefenderEffect();
                yield return GManager.instance.OnTargetArrow(
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(), 
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(), 
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard,
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard);
                #endregion

                yield return null;
            }
        }

        return cardEffects;
    }

    
}
