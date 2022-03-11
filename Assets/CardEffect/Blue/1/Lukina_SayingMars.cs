using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Lukina_SayingMars : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("裏剣 ファルシオン",(enemyUnit, Power) => Power + 20, (unit) => unit ==card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.Dragon), PowerUpByEnemy.Mode.Attacking,card);
        cardEffects.Add(powerUpByEnemy);

        UnitNamesChangeClass unitNamesChangeClass = new UnitNamesChangeClass();
        unitNamesChangeClass.SetUpICardEffect("英雄王の名","", null, null, -1, false,card);
        unitNamesChangeClass.SetUpUnitNamesChangeClass((cardSource, UnitNames) => { UnitNames.Add("マルス"); return UnitNames; }, (cardSource) => card);
        cardEffects.Add(unitNamesChangeClass);

        return cardEffects;
    }

    #region 英雄の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("英雄の紋章", "Hero Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

            IEnumerator ActivateCoroutine()
            {
                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, CanStrikeModifyCondition, false);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => strikeModifyClass);

                bool CanStrikeModifyCondition(Unit unit)
                {
                    if (unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character != null)
                    {
                        if (unit.Character.Owner == card.Owner)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                yield return null;
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.Blue))
                                {
                                    return true;
                                }
                            }

                        }
                    }
                }

                return false;
            }
        }


        return supportEffects;
    }
    #endregion
}

