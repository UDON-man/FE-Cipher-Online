using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class Soware_RedEyeOx : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("紅と碧の絆", null, null, -1, false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, CanPowerUpCondition);
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
        canNotAttackClass.SetUpICardEffect("聖盾", null, null, -1, false);
        canNotAttackClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(canNotAttackClass);

        canNotAttackClass.SetUpCanNotAttackClass(
            (AttackingUnit) => 
            AttackingUnit.Character.Owner != this.card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit) 
            && (AttackingUnit.Weapons.Contains(Weapon.Bow)|| AttackingUnit.Weapons.Contains(Weapon.MagicBook)|| AttackingUnit.Weapons.Contains(Weapon.DragonStone)), 
            (DefendingUnit) => DefendingUnit.Character.Owner == this.card.Owner);

        
        

        return cardEffects;
    }
}
