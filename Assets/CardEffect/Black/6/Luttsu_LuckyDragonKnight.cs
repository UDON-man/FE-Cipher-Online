using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Luttsu_LuckyDragonKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("強運の風", "Auspicious Wind", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if (Unit.Character != null)
                            {
                                if (Unit.Character == card)
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
                List<CardSource> TopCards = new List<CardSource>();
                List<CardSource> LeftCards = new List<CardSource>();

                for (int i = 0; i < 2; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if (card.Owner.LibraryCards.Count > 0)
                    {
                        CardSource topCard = card.Owner.LibraryCards[0];
                        TopCards.Add(topCard);
                        card.Owner.LibraryCards.Remove(topCard);
                    }

                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));
                }

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => true,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                    Message: "Select a card to discard.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Custom,
                    CustomRootCardList: TopCards,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator SelectCardCoroutine(CardSource cardSource)
                {
                    CardObjectController.AddTrashCard(cardSource);
                    yield return null;
                }

                IEnumerator AfterSelectCardCoroutine(List<CardSource> targetCards)
                {
                    foreach (CardSource cardSource in TopCards)
                    {
                        if (!targetCards.Contains(cardSource))
                        {
                            LeftCards.Add(cardSource);
                        }
                    }

                    yield return null;
                }

                if (LeftCards.Count > 0)
                {
                    foreach(CardSource cardSource in LeftCards)
                    {
                        card.Owner.LibraryCards.Insert(0, cardSource);
                    }
                }
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

