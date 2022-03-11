using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quan_PrinceOfLeonster : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("レンスターの戦術", "Leonster's Tactics", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1,false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit.Character != null && GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                    {
                        if(GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                        {
                            if(GManager.instance.turnStateMachine.AttackingUnit.Character.Owner.GetBackUnits().Contains(GManager.instance.turnStateMachine.AttackingUnit))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                Unit DefendingUnit = GManager.instance.turnStateMachine.DefendingUnit;

                if(DefendingUnit != null)
                {
                    if(DefendingUnit.Character != null)
                    {
                        if(DefendingUnit.Character.Owner = card.Owner)
                        {
                            PowerModifyClass powerUpClass = new PowerModifyClass();
                            powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == DefendingUnit, true);
                            DefendingUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);
                        }
                    }
                }

                yield return null;
            }
        }

        return cardEffects;
    }

    #region 攻撃の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("攻撃の紋章", "Attack Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
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
                                return true;
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

