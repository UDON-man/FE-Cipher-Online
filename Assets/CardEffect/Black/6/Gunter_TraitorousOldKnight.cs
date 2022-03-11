using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Gunter_TraitorousOldKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();
        
        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("透魔王の支配", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool PowerUpCondition(Unit unit)
        {
            if (unit == card.UnitContainingThisCharacter())
            {
                if (card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) >= 5)
                {
                    return true;
                }
            }

            return false;
        }

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("透魔王の支配", "", null, null, -1, false, card);
        weaponChangeClass.SetUpWeaponChangeClass((CardSource, Weapons) => { Weapons.Add(Weapon.Dragon); return Weapons; }, CanWeaponChangeCondition);
        cardEffects.Add(weaponChangeClass);

        bool CanWeaponChangeCondition(CardSource cardSource)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (cardSource == card.UnitContainingThisCharacter().Character)
                {
                    if (card.Owner.BondCards.Count((_cardSource) => _cardSource.IsReverse) >= 5)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        CanNotDestroyedBySkillClass canNotDestroyedBySkillClass = new CanNotDestroyedBySkillClass();
        canNotDestroyedBySkillClass.SetUpICardEffect("竜鱗", "", null, null, -1, false, card);
        canNotDestroyedBySkillClass.SetUpCanNotDestroyedBySkillClass((unit) => unit == card.UnitContainingThisCharacter(),(cardEffect) => true);
        cardEffects.Add(canNotDestroyedBySkillClass);

        CanNotDestroyedByCostClass canNotDestroyedByCostClass = new CanNotDestroyedByCostClass();
        canNotDestroyedByCostClass.SetUpICardEffect("竜鱗", "", null, null, -1, false, card);
        canNotDestroyedByCostClass.SetUpCanNotDestroyedByCostClass((unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(canNotDestroyedByCostClass);

        return cardEffects;
    }
}
