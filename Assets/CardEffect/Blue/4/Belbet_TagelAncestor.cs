using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Belbet_TagelAncestor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanAttackTargetUnitRegardlessRangeClass canAttackTargetUnitRegardlessRangeClass = new CanAttackTargetUnitRegardlessRangeClass();
        canAttackTargetUnitRegardlessRangeClass.SetUpICardEffect("狩りの呼吸",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition },-1,false);
        canAttackTargetUnitRegardlessRangeClass.SetUpCanAttackTargetUnitRegardlessRangeClass((AttackingUnit) => AttackingUnit == card.UnitContainingThisCharacter() && AttackingUnit.Character.Owner.GetFrontUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit.Character.Owner.GetBackUnits().Contains(DefendingUnit));
        cardEffects.Add(canAttackTargetUnitRegardlessRangeClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.BondCards.Count % 2 == 0)
            {
                return true;
            }

            return false;
        }

        return cardEffects;
    }
}