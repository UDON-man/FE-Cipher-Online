using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Aqua_DawnSingPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("運命の詠み歌", "Song of Fate", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                List<CardSource> TopCards = new List<CardSource>();
                List<CardSource> LeftCards = new List<CardSource>();

                for (int i = 0; i < 3; i++)
                {
                    yield return ContinuousController.instance.StartCoroutine(Refresh.RefreshCheck(card.Owner));

                    if(card.Owner.LibraryCards.Count > 0)
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

                if(LeftCards.Count > 0)
                {
                    selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => true,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: (cardSource) => SelectCardCoroutine1(cardSource),
                    AfterSelectCardCoroutine: null,
                    Message: "Select cards to place on top of deck.\n(The one with the smaller number goes down)",
                    MaxCount: LeftCards.Count,
                    CanEndNotMax: false,
                    isShowOpponent: false,
                    mode: SelectCardEffect.Mode.Custom,
                    root: SelectCardEffect.Root.Custom,
                    CustomRootCardList: LeftCards,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    IEnumerator SelectCardCoroutine1(CardSource cardSource)
                    {
                        card.Owner.LibraryCards.Insert(0, cardSource);
                        yield return null;
                    }
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("異邦の王女","",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition1 },-1,false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        bool CanUseCondition1(Hashtable hashtable)
        {
            if(card.Owner.BondCards.Count((cardSource) => cardSource.IsReverse) >= 2)
            {
                return true;
            }

            return false;
        }

        AddHasCCClass addHasCCClass = new AddHasCCClass();
        addHasCCClass.SetUpICardEffect("透魔への白道", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        addHasCCClass.SetUpAddHasCCClass((cardSource) => cardSource == card,(unit) => true);
        cardEffects.Add(addHasCCClass);

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("透魔への白道", "",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false,card);
        changeCCCostClass.SetUpChangeCCCostClass((cardSource,unit,CCCost) => 2,(cardSource) => cardSource == card);
        cardEffects.Add(changeCCCostClass);

        bool CanUseCondition2(Hashtable hashtable)
        {
            if (card != null)
            {
                if (card.Owner.BondCards.Count((cardSource) => !cardSource.IsReverse && cardSource.cardColors.Contains(CardColor.Black)) >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        return cardEffects;
    }
}

