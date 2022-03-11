using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sophia_ProPhetOfTheHiddenVillage : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("予言", "Prophecy",new List<Cost>() { new TapCost() }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> TopCards = new List<CardSource>();
                List<CardSource> LeftCards = new List<CardSource>();

                for (int i = 0; i < 1; i++)
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
                    CanNoSelect: () => true,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                    AfterSelectCardCoroutine: (targetCards) => AfterSelectCardCoroutine(targetCards),
                    Message: "Select cards to discard.",
                    MaxCount: TopCards.Count,
                    CanEndNotMax: true,
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
                    card.Owner.LibraryCards.Insert(0, LeftCards[0]);

                    GManager.instance.commandText.OpenCommandText("The card was not discarded.");
                    yield return new WaitForSeconds(1f);

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
                }

                else
                {
                    GManager.instance.commandText.OpenCommandText("The card was discarded.");
                    yield return new WaitForSeconds(1f);

                    GManager.instance.commandText.CloseCommandText();
                    yield return new WaitWhile(() => GManager.instance.commandText.gameObject.activeSelf);
                }
            }
        }

        return cardEffects;
    }

    #region 暗闇の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("暗闇の紋章", "Darkness Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
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
                                if (card.Owner.Enemy.HandCards.Count >= 5)
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
                if (card.Owner.Enemy.HandCards.Count >= 5)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                                    SelectPlayer: card.Owner.Enemy,
                                    CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                                    CanTargetCondition_ByPreSelecetedList: null,
                                    CanEndSelectCondition: null,
                                    MaxCount: 1,
                                    CanNoSelect: false,
                                    CanEndNotMax: false,
                                    isShowOpponent: true,
                                    SelectCardCoroutine: null,
                                    AfterSelectCardCoroutine: null,
                                    mode: SelectHandEffect.Mode.Discard,
                                    cardEffect: activateClass_Support);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return supportEffects;
    }
    #endregion
}

