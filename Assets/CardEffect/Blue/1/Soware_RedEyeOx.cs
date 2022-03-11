using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Soware_RedEyeOx : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("紅と碧の絆", "",null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (unit == GManager.instance.turnStateMachine.AttackingUnit || unit == GManager.instance.turnStateMachine.DefendingUnit)
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (card.Owner.SupportCards.Count((_cardSource) => _cardSource.UnitNames.Contains("ソール")) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("聖盾", "",null, null, -1, false,card);
        canNotAttackClass.SetCCS(card.UnitContainingThisCharacter());
        canNotAttackClass.SetUpCanNotAttackClass(AttackingCondition, DefendingCondition);
        cardEffects.Add(canNotAttackClass);

        bool AttackingCondition(Unit AttackingUnit)
        {
            if(AttackingUnit != null)
            {
                if (AttackingUnit.Character != null)
                {
                    if (AttackingUnit.Character.Owner != card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit))
                    {
                        if (AttackingUnit.Weapons.Contains(Weapon.Bow) || AttackingUnit.Weapons.Contains(Weapon.MagicBook) || AttackingUnit.Weapons.Contains(Weapon.DragonStone))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        bool DefendingCondition(Unit DefendingUnit)
        {
            if(DefendingUnit != null)
            {
                if (DefendingUnit.Character != null)
                {
                    if (DefendingUnit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        return cardEffects;
    }
}
