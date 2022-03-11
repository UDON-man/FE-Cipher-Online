using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Senerio_CoolWindSanbo : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("最適な戦略", "Optimal Strategy",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() ,1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 3).Draw());

                if (card.Owner.HandCards.Count >= 2)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: 2,
                        CanNoSelect: false,
                        CanEndNotMax: false,
                        isShowOpponent: true,
                        SelectCardCoroutine: null,
                        AfterSelectCardCoroutine: null,
                        mode: SelectHandEffect.Mode.Discard,
                        cardEffect: activateClass);

                    yield return StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}

