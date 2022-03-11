using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sarya_LoveDarkUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ルイン", "Ruin", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("サーリャ")) }, new List<Func<Hashtable, bool>>(), 1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.Enemy.HandCards.Count == 0)
                {
                    yield break;
                }

                SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                int maxCount = 2;

                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.UnitContainingThisCharacter().IsClassChanged())
                    {
                        maxCount = 3;
                    }
                }

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
                        cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("禁断の呪い","", null, null, -1,false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter() && card.Owner.Enemy.HandCards.Count == 0, true);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}