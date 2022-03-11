using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Arvis_HolyFlameInferitor : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("炎魔法 ファラフレイム", "Fire Magic: Valflame",new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("アルヴィス")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.Owner != card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                if(card.Owner.Enemy.BondCards.Count > 5)
                {
                    List<CardSource> LeftCards = new List<CardSource>();

                    SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => true,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => false,
                        SelectCardCoroutine: SelectCardCoroutine,
                        AfterSelectCardCoroutine: null,
                        Message: "Select your bond cards you want to keep.",
                        MaxCount: 5,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.Custom,
                        root: SelectCardEffect.Root.Bond,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer:card.Owner.Enemy,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                    
                    IEnumerator SelectCardCoroutine(CardSource cardSource)
                    {
                        if(!LeftCards.Contains(cardSource))
                        {
                            LeftCards.Add(cardSource);
                        }

                        yield return null;
                    }

                    if(LeftCards.Count == 5)
                    {
                        List<CardSource> DiscardCards = new List<CardSource>();

                        foreach (CardSource cardSource in card.Owner.Enemy.BondCards)
                        {
                            if (!LeftCards.Contains(cardSource))
                            {
                                DiscardCards.Add(cardSource);
                            }
                        }

                        foreach (CardSource cardSource in DiscardCards)
                        {
                            cardSource.Owner.BondCards.Remove(cardSource);
                            CardObjectController.AddTrashCard(cardSource);
                        }

                        yield return StartCoroutine(card.Owner.Enemy.bondObject.SetBond_Skill(card.Owner.Enemy));
                    }
                }
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("理想の為の犠牲", "A Sacrifice for Ideals",new List<Cost>() , new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass1);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(null,card))
                {
                    if(card.Owner.BondCards.Count > 0)
                    {
                        return true;
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
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select your bond card to discard.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.DiscardFromBond,
                    root: SelectCardEffect.Root.Bond,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                yield return StartCoroutine(card.Owner.bondObject.SetBond_Skill(card.Owner));

                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}