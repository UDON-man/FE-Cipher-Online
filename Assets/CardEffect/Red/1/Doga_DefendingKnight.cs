using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Doga_DefendingKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("重装の心得", (enemyUnit, Power) => Power + 20, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending);
        cardEffects.Add(powerUpByEnemy);

        return cardEffects;
    }

    #region 防御の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support[0].SetUpICardEffect("防御の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Defense Emblem";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.DefendingUnit && unit.Character.Owner == card.Owner);
                GManager.instance.turnStateMachine.DefendingUnit.UntilEndBattleEffects.Add(powerUpClass);

                yield return null;
            }
        }
        

        return supportEffects;
    }
    #endregion
}
