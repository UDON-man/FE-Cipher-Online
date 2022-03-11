using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Thito_StormyFlier : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ひるがえる白翼", "Fluttering White Wings", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                            {
                                if(card.UnitContainingThisCharacter() != card.Owner.Lord)
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
                List<CardSource> cardSources = new List<CardSource>();

                if(card.UnitContainingThisCharacter().Characters.Count == 1)
                {
                    cardSources.Add(card.UnitContainingThisCharacter().Character);
                }

                else
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => true,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine1(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select the order to place on top of deck.\n(The one with the smaller number goes down)",
                    MaxCount: card.UnitContainingThisCharacter().Characters.Count,
                    CanEndNotMax: false,
                    isShowOpponent: false,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Custom,
                    CustomRootCardList: card.UnitContainingThisCharacter().Characters,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    IEnumerator SelectCardCoroutine1(CardSource cardSource)
                    {
                        cardSources.Add(cardSource);
                        yield return null;
                    }
                }

                ContinuousController.instance.StartCoroutine(GManager.instance.GetComponent<Effects>().ShowCardEffect(cardSources, "Deck Top Cards", true));

                Unit unit = card.UnitContainingThisCharacter();
                unit.Characters = new List<CardSource>();

                foreach (CardSource cardSource in cardSources)
                {
                    CardObjectController.RemoveField(cardSource);
                    card.Owner.LibraryCards.Insert(0, cardSource);
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
