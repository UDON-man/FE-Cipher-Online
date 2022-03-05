using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Aqua_DawnSingPrincess : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[0].SetUpICardEffect("運命の詠み歌", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), 1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Song of Fate";
            }

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
                    CanLookReverseCard: true);

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
                    CanLookReverseCard: true);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                    IEnumerator SelectCardCoroutine1(CardSource cardSource)
                    {
                        card.Owner.LibraryCards.Insert(0, cardSource);
                        yield return null;
                    }
                }
            }


        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("異邦の王女",new List<Cost>(),new List<Func<Hashtable, bool>>() { CanUseCondition1 },-1,false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter());
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
        addHasCCClass.SetUpICardEffect("透魔への白道", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        addHasCCClass.SetUpAddHasCCClass((cardSource) => cardSource == this.card,(unit) => true);
        cardEffects.Add(addHasCCClass);

        ChangeCCCostClass changeCCCostClass = new ChangeCCCostClass();
        changeCCCostClass.SetUpICardEffect("透魔への白道", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition2 }, -1, false);
        changeCCCostClass.SetUpChangeCCCostClass((cardSource,unit,CCCost) => 2,(cardSource) => cardSource == this.card);
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

