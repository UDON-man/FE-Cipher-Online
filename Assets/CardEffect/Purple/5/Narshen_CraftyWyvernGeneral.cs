using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Narshen_CraftyWyvernGeneral : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            activateClass[0].SetUpICardEffect("権謀術数", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Machiavellism";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable))
                {
                    if (hashtable != null)
                    {
                        if (hashtable.ContainsKey("Unit"))
                        {
                            if (hashtable["Unit"] is Unit)
                            {
                                Unit Unit = (Unit)hashtable["Unit"];

                                if (Unit == card.UnitContainingThisCharacter())
                                {
                                    if (card.Owner.HandCards.Count < card.Owner.Enemy.HandCards.Count)
                                    {
                                        if (card.Owner.Enemy.HandCards.Count > 0)
                                        {
                                            return true;
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

                int maxCount = 1;

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
                        mode: SelectHandEffect.Mode.PutLibraryTop);

                yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
            }
        }

        InvalidationClass invalidationClass = new InvalidationClass();
        invalidationClass.SetUpICardEffect("策士の哄笑", null, new List<Func<Hashtable, bool>>() , -1, false);
        invalidationClass.SetUpInvalidationClass(InvalidateCondition);
        cardEffects.Add(invalidationClass);

        bool InvalidateCondition(ICardEffect cardEffect)
        {
            if(this.IsExistOnField(null))
            {
                if (cardEffect.card() != null)
                {
                    if (cardEffect.card().Owner == card.Owner.Enemy)
                    {
                        if (cardEffect.card().UnitContainingThisCharacter() != null || cardEffect.card().Owner.SupportCards.Contains(cardEffect.card()))
                        {
                            if (cardEffect.isSupportSkill)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}