using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rete_ProudBattleCat : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotSupportClass canNotSupportClass = new CanNotSupportClass();
        canNotSupportClass.SetUpCanNotSupportClass((cardSource) => !cardSource.Weapons.Contains(Weapon.Beast) , (unit) => unit == card.UnitContainingThisCharacter());
        canNotSupportClass.SetUpICardEffect("ベオク嫌い","", null, null, -1, false,card);
        cardEffects.Add(canNotSupportClass);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("化身","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false,card);
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
            if(card.UnitContainingThisCharacter() != null)
            {
               if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    return true;
                }
            }
            return false;
        }

        return cardEffects;
    }
}
