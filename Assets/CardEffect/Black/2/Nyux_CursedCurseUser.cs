using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Nyux_CursedCurseUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("幼子の姿", "Youthful Figure",new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition  }, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(card.Owner.HandCards.Count >= 6)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.HandCards.Count >= 6)
                {
                    int DiscardCount = card.Owner.HandCards.Count - 5;

                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    selectHandEffect.SetUp(
                        SelectPlayer: card.Owner,
                        CanTargetCondition: (cardSource) => cardSource.Owner.HandCards.Contains(cardSource),
                        CanTargetCondition_ByPreSelecetedList: null,
                        CanEndSelectCondition: null,
                        MaxCount: DiscardCount,
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