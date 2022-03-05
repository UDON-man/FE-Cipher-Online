using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Elfie_PowerfulHeavyKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpICardEffect("エリーゼ隊のエルフィ", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("エリーゼ隊のエルフィ", (enemyUnit, Power) => Power + 30, (unit) => unit == this.card.UnitContainingThisCharacter(), (enemyUnit) => !enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending);
        cardEffects.Add(powerUpByEnemy);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit.Character.UnitNames.Contains("エリーゼ")) > 0)
            {
                return true;
            }

            return false;
        }

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
