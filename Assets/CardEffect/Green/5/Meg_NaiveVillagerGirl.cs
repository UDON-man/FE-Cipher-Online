using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Meg_NaiveVillagerGirl : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackedAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("メグの盾", "Meg's Shield", new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit != card.UnitContainingThisCharacter())
                                {
                                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                                    {
                                        if(GManager.instance.turnStateMachine.DefendingUnit.Character.PlayCost == 1)
                                        {
                                            return true;
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
                #region 旧防御ユニットのエフェクトを削除
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.OffAttackerDefenderEffect();
                GManager.instance.OffTargetArrow();
                #endregion

                //防御ユニットを更新
                GManager.instance.turnStateMachine.DefendingUnit = card.UnitContainingThisCharacter();

                #region 新防御ユニットのエフェクトを表示
                GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.SetDefenderEffect();
                yield return GManager.instance.OnTargetArrow(
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard.GetLocalCanvasPosition(),
                    GManager.instance.turnStateMachine.AttackingUnit.ShowingFieldUnitCard,
                    GManager.instance.turnStateMachine.DefendingUnit.ShowingFieldUnitCard);
                #endregion

                yield return null;
            }
        }

        return cardEffects;
    }

    #region 防御の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("防御の紋章", "Defense Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
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

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == GManager.instance.turnStateMachine.DefendingUnit && unit.Character.Owner == card.Owner, true);
                GManager.instance.turnStateMachine.DefendingUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }


        return supportEffects;
    }
    #endregion
}
