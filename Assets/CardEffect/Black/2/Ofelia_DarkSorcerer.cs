using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ofelia_DarkSorcerer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("乙女心の躍動","", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition, true);
        powerUpClass.SetCCS(card.UnitContainingThisCharacter());
        cardEffects.Add(powerUpClass);

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
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.MagicBook)) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
        strikeModifyClass.SetUpICardEffect("ロイヤルブラッディ・マーク","", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
        strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(),false);
        cardEffects.Add(strikeModifyClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() || GManager.instance.turnStateMachine.DefendingUnit == card.UnitContainingThisCharacter())
                {
                    if (card.Owner.SupportCards.Count((cardSource) => cardSource.cEntity_Base.UnitName.Contains("オーディン")) > 0)
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

