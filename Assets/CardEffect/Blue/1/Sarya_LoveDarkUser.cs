using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sarya_LoveDarkUser : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            if (card.Owner.Enemy.HandCards.Count > 0)
            {
                activateClass[0].SetUpICardEffect("ルイン", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("サーリャ")) }, null, 1, false);
                activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
                cardEffects.Add(activateClass[0]);

                if (ContinuousController.instance.language == Language.ENG)
                {
                    activateClass[0].EffectName = "Ruin";
                }

                IEnumerator ActivateCoroutine()
                {
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
                            mode: SelectHandEffect.Mode.Discard);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
                
            }
        }

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("禁断の呪い", null, null, -1,false);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 20, (unit) => unit == card.UnitContainingThisCharacter() && card.Owner.Enemy.HandCards.Count == 0);
        cardEffects.Add(powerUpClass);

        return cardEffects;
    }
}