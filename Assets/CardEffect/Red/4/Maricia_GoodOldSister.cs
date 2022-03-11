using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Maricia_GoodOldSister : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ライブ", "Heal", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("マリーシア"),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to add to hand.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.AddHand,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
            }
        }

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
        rangeUpClass.SetUpICardEffect("マルス様のためなら!","", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
        cardEffects.Add(rangeUpClass);

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("マルス")) > 0)
            {
                return true;
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

