using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elincia_EsteemedQueen : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("女王の激励", "Queen's Encouragement", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Unit targetUnit = null;

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    targetUnit = unit;

                    yield return null;
                }

                if (targetUnit != null)
                {
                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: CanTargetCondition,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => true,
                        SelectCardCoroutine: (cardSource) => SelectCardCoroutine(cardSource),
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card to stack on top.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.Custom,
                        root: SelectCardEffect.Root.Library,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    bool CanTargetCondition(CardSource cardSource)
                    {
                        foreach (string UnitName in targetUnit.Character.UnitNames)
                        {
                            if (cardSource.UnitNames.Contains(UnitName))
                            {
                                if(cardSource.cardColors.Contains(CardColor.Green))
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                    }

                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass);
                        yield return ContinuousController.instance.StartCoroutine(new IPlayUnit(cardSource, targetUnit, false, true, hashtable, false).PlayUnit());
                    }
                }

            }
        }

        else if(timing == EffectTiming.OnLevelUpAnyone|| timing == EffectTiming.OnGrowAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("宝剣 アミーテ", "Sacred Treasure Amiti", new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1,true,card);
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

                            if (Unit.Character.Owner == card.Owner)
                            {
                                if (Unit != card.UnitContainingThisCharacter())
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
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(card.UnitContainingThisCharacter().UnTap(hashtable));
            }
        }

        return cardEffects;
    }
}