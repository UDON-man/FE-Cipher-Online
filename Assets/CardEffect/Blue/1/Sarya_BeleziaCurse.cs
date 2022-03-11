using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sarya_BeleziaCurse : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            if (card.Owner.Enemy.HandCards.Count > 0)
            {
                ActivateClass activateClass = new ActivateClass();
                activateClass.SetUpICardEffect("ミィル", "Flux", new List<Cost>() { new TapCost(), new ReverseCost(2, (cardSource) => true) }, null, -1, false,card);
                activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass);

                IEnumerator ActivateCoroutine()
                {
                    if(card.Owner.Enemy.HandCards.Count > 0)
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
                            cardEffect: activateClass);

                        yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                    }
                }
                
            }
        }

        return cardEffects;
    }
}