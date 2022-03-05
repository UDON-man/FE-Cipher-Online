using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Thito_StormyFlier : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndAttackAnyone)
        {
            activateClass[0].SetUpICardEffect("ひるがえる白翼", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Fluttering White Wings";
            }

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
                    CanLookReverseCard: true);

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
    public override List<ICardEffect> SupportEffects(EffectTiming timing)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnSetSupport)
        {
            activateClass_Support[0].SetUpICardEffect("天空の紋章", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass_Support[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass_Support[0].EffectName = "Flying Emblem";
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
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return supportEffects;
    }
    #endregion
}
