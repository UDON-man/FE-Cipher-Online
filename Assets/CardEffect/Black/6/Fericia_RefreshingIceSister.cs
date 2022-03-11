using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Fericia_RefreshingIceSister : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("氷の姉妹", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, PowerUpCondition, true);
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
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.UnitNames.Contains("フローラ")) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        WeaponChangeClass weaponChangeClass = new WeaponChangeClass();
        weaponChangeClass.SetUpICardEffect("暗器修行中","",null,null,-1,false,card);
        weaponChangeClass.SetUpWeaponChangeClass((CardSource, Weapons) => { Weapons.Add(Weapon.DarkWeapon); return Weapons; }, CanWeaponChangeCondition);
        cardEffects.Add(weaponChangeClass);

        bool CanWeaponChangeCondition(CardSource cardSource)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (cardSource == card.UnitContainingThisCharacter().Character)
                {
                    return true;
                }
            }

            return false;
        }

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("暗器修行中", "", null, null, -1, false, card);
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, CanRangeUpCondition);
        cardEffects.Add(rangeUpClass);

        bool CanRangeUpCondition(Unit unit)
        {
            if(IsExistOnField(null,card))
            {
                if(unit == card.UnitContainingThisCharacter())
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }

    #region 祈りの紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("祈りの紋章", "Miracle Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.gameContext.NonTurnPlayer == card.Owner)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                                {
                                    return true;
                                }
                            }
                        }

                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                CanNotCriticalClass canNotCriticalClass = new CanNotCriticalClass();
                canNotCriticalClass.SetUpCanNotCriticalClass((unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner != card.Owner);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => canNotCriticalClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}
