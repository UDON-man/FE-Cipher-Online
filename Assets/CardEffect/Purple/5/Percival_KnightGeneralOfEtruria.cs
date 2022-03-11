using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Percival_KnightGeneralOfEtruria : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("騎士の中の騎士", "",null, new List<Func<Hashtable, bool>>() { (hashtable) => IsExistOnField(hashtable,card) }, -1, false,card);
        canNotAttackClass.SetUpCanNotAttackClass((AttackingUnit) => AttackingUnit.Character.Owner != card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit), (DefendingUnit) => DefendingUnit.Character.Owner == card.Owner && DefendingUnit.Character.cEntity_EffectController.GetAllSupportEffects().Count((cardEffect) => !cardEffect.IsInvalidate) > 0);
        cardEffects.Add(canNotAttackClass);

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("騎士を統べる者","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, PowerUpCondition, true);
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
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.Horse)) > 0)
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

    #region 軍将の勇槍
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("軍将の勇槍", "General's Brave Lance", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.AttackingUnit.Weapons.Contains(Weapon.Horse))
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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character.Owner == card.Owner, true);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return supportEffects;
    }
    #endregion
}

