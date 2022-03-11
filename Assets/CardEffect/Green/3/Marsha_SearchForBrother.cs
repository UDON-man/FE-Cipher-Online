using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class Marsha_SearchForBrother : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpByEnemy powerUpByEnemy = new PowerUpByEnemy();
        powerUpByEnemy.SetUpPowerUpByEnemyWeapon("天馬の護り", (enemyUnit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter(), (enemyUnit) => enemyUnit.Weapons.Contains(Weapon.MagicBook), PowerUpByEnemy.Mode.Defending,card);
        cardEffects.Add(powerUpByEnemy);

        SupportPowerUpClass supportPowerUpClass = new SupportPowerUpClass();
        supportPowerUpClass.SetUpICardEffect("天馬の叫び","", null, null, -1, false,card);
        supportPowerUpClass.SetUpSupportPowerUpClass((cardSource, SupportPower) => SupportPower + 10, ChangeSupportPowerCondition);
        supportPowerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(supportPowerUpClass);

        bool ChangeSupportPowerCondition(CardSource cardSource)
        {
            if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner || GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                {
                    if (card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.AttackingUnit && card.UnitContainingThisCharacter() != GManager.instance.turnStateMachine.DefendingUnit)
                    {
                        if (cardSource.Weapons.Contains(Weapon.Wing))
                        {
                            if (cardSource.Owner == card.Owner)
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            return false;
        }

        return cardEffects;
    }
}