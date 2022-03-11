using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Tsubasa_DreamingHighSchoolStudent : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("エアリアルダンス", "Aerial Dance", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("シーダ")) }, new List<Func<Hashtable, bool>>() , -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: card.Owner.FieldUnit.Count,
                CanNoSelect: true,
                CanEndNotMax: true,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Move,
                cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }

    #region 天空の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("天空の紋章", "Flying Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
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
                                if (card.Owner.FieldUnit.Count((unit) => unit != GManager.instance.turnStateMachine.AttackingUnit) > 0)
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != GManager.instance.turnStateMachine.AttackingUnit,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass_Support);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return supportEffects;
    }
    #endregion
}