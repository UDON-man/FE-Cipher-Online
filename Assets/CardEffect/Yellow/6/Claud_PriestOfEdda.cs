using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Claud_PriestOfEdda : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("聖杖 バルキリー", "Holly Staff Valkyria",new List<Cost>() { new TapCost(), new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                bool selected = false;

                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: (cardSource) => cardSource.Owner.BondCards.Contains(cardSource) && !cardSource.IsReverse && cardSource.PlayCost <= 2 && cardSource.cardColors.Contains(CardColor.Yellow) && cardSource.CanPlayAsNewUnit(),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: (cardSources) => AfterSelectCardCoroutine(cardSources),
                    Message: "Select a card in your bond area to deploy.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Deploy,
                    root: SelectCardEffect.Root.Bond,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                IEnumerator AfterSelectCardCoroutine(List<CardSource> cardSources)
                {
                    selected = cardSources.Count > 0;
                    yield return null;
                }

                if(selected)
                {
                    yield return StartCoroutine(card.Owner.bondObject.SetBond_Skill(card.Owner));
                    GManager.instance.turnStateMachine.IsSelecting = true;
                    yield return new WaitForSeconds(0.8f);

                    selectCardEffect.SetUp(
                        CanTargetCondition: (cardSource) => cardSource.CanSetBondThisCard,
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        CanNoSelect: () => true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        Message: "Select a card in your retreat area to add to bond.",
                        MaxCount: 1,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        mode: SelectCardEffect.Mode.SetFaceBond,
                        root: SelectCardEffect.Root.Trash,
                        CustomRootCardList: null,
                        CanLookReverseCard: true,
                        SelectPlayer: card.Owner,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}

