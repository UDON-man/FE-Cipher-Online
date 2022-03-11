using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Kiria_CoolOrCute : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("迷路", "Maze", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("黒乃霧亜") || cardSource.UnitNames.Contains("サーリャ")) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                if (card.Owner.Enemy.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    int maxCount = 2;

                    if (card.Owner.Enemy.HandCards.Count < maxCount)
                    {
                        maxCount = card.Owner.Enemy.HandCards.Count;
                    }

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner.Enemy,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: maxCount,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Discard,
                        cardEffect: activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("パステルアワー", "Pastel Carousel", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("黒乃霧亜") || cardSource.UnitNames.Contains("サーリャ")) }, null, 1, false,card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine2()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        return cardEffects;
    }
}