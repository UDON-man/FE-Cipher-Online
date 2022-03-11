using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Yurisizu_CirmiaSubLeader : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnGrowAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("裏の仕事", "Back to Work", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if (hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if(Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner)
                                {
                                    if (Unit != card.UnitContainingThisCharacter())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                yield return ContinuousController.instance.StartCoroutine(new IDraw(card.Owner, 1).Draw());
            }
        }

        else if (timing == EffectTiming.OnDiscardHand)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("クリミア一の策士", "Crimea's Tactician", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.TrashCards.Contains(card))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Card"))
                        {
                            if (hashtable["Card"] is CardSource)
                            {
                                if (hashtable.ContainsKey("cardEffect"))
                                {
                                    if (hashtable["cardEffect"] is ICardEffect)
                                    {
                                        ICardEffect cardEffect = (ICardEffect)hashtable["cardEffect"];
                                        CardSource cardSource = (CardSource)hashtable["Card"];

                                        if (cardEffect != null && cardSource != null)
                                        {
                                            if (cardEffect.card() != null)
                                            {
                                                if (cardEffect.card().Owner == card.Owner.Enemy)
                                                {
                                                    if (cardSource == card)
                                                    {
                                                        if (card.Owner.BondCards.Count((_cardSource) => !_cardSource.IsReverse && _cardSource.cardColors.Contains(CardColor.Green)) > 0)
                                                        {
                                                            if(card.Owner.Enemy.HandCards.Count > 0)
                                                            {
                                                                return true;
                                                            }
                                                            
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                        }
                    }
                }


                return false;
            }

            IEnumerator ActivateCoroutine()
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

        return cardEffects;
    }
}