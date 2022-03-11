using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Pieri_PureKillInstinct : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ピエリもお返しなの!", "Peri wants to return the favour too!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Defender"))
                    {
                        if (hashtable["Defender"] is Unit)
                        {
                            Unit Defender = (Unit)hashtable["Defender"];

                            if (Defender != null)
                            {
                                if (Defender.Character != null)
                                {
                                    if (Defender.Character == card)
                                    {
                                        if(GManager.instance.turnStateMachine.AttackingUnit != null)
                                        {
                                            if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                                            {
                                                if (card.Owner.Enemy.GetBackUnits().Contains(GManager.instance.turnStateMachine.AttackingUnit))
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if (card.Owner.Enemy.GetBackUnits().Contains(GManager.instance.turnStateMachine.AttackingUnit))
                        {
                            Hashtable hashtable1 = new Hashtable();
                            hashtable1.Add("cardEffect", activateClass);
                            hashtable1.Add("Unit", new Unit(GManager.instance.turnStateMachine.AttackingUnit.Characters));
                            yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(GManager.instance.turnStateMachine.AttackingUnit, 1, BreakOrbMode.Hand, hashtable1).Destroy());
                        }
                    }
                }
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