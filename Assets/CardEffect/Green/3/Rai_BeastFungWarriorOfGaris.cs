using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Rai_BeastFungWarriorOfGaris : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass1 = new PowerUpClass();
        powerUpClass1.SetUpICardEffect("獣牙の絆", null, null, -1, false);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition);
        cardEffects.Add(powerUpClass1);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == unit || GManager.instance.turnStateMachine.DefendingUnit == unit)
                    {
                        if (card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.AttackingUnit || card.UnitContainingThisCharacter() == GManager.instance.turnStateMachine.DefendingUnit)
                        {
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.Beast)) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("化身", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
        powerUpClass.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass);

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("化身", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter(), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
        canAttackTargetUnitRegardlessRangeClass.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(canAttackTargetUnitRegardlessRangeClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    return true;
                }
            }
            return false;
        }

        return cardEffects;
    }
}
