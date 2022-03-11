using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Roy_FeresNobleBoy : CEntity_Effect
{
    #region 支援スキル
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        #region 英雄の紋章
        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass_Support.SetUpICardEffect("英雄の紋章", "Hero Emblem",null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            supportEffects.Add(activateClass_Support);

            IEnumerator ActivateCoroutine()
            {
                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, CanStrikeModifyCondition, false);
                GManager.instance.turnStateMachine.AttackingUnit.UntilEndBattleEffects.Add((_timing) => strikeModifyClass);

                bool CanStrikeModifyCondition(Unit unit)
                {
                    if (unit == GManager.instance.turnStateMachine.AttackingUnit && unit.Character != null)
                    {
                        if (unit.Character.Owner == card.Owner)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                yield return null;
            }

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
                                if (GManager.instance.turnStateMachine.AttackingUnit.Character.cardColors.Contains(CardColor.Purple))
                                {
                                    return true;
                                }
                            }

                        }
                    }
                }

                return false;
            }
        }
        #endregion

        #region 希望の紋章
        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support1 = new ActivateClass();
            activateClass_Support1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            activateClass_Support1.SetUpICardEffect("希望の紋章", "Hope Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, false,card);
            supportEffects.Add(activateClass_Support1);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.Owner == card.Owner)
                            {
                                if (GManager.instance.turnStateMachine.DefendingUnit.Character.cardColors.Contains(CardColor.Purple))
                                {
                                    if(card.Owner.OrbCards.Count > 0)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine1()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => true,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => true,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select a orb card to see the surface.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: false,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Orb,
                    CustomRootCardList: null,
                    CanLookReverseCard: false,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass_Support1);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    if (card.Owner.isYou)
                    {
                        ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(new List<CardSource>() { cardSource }, "Orb Card", true));
                    }

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
        #endregion

        return supportEffects;
    }
    #endregion
}
