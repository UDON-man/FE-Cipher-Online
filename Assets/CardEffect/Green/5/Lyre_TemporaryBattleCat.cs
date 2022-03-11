using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Lyre_TemporaryBattleCat : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass1 = new PowerModifyClass();
        powerUpClass1.SetUpICardEffect("憧れの隊長", "",null, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
        powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass1);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((_unit) => _unit != card.UnitContainingThisCharacter() && _unit.Weapons.Contains(Weapon.Beast)) > 0)
            {
                return true;
            }

            return false;
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("化身","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        powerUpClass.SetLvS(card.UnitContainingThisCharacter(), 2);
        cardEffects.Add(powerUpClass);

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("化身","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
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
