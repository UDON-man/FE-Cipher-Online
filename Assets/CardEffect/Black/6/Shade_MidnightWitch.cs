using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Shade_MidnightWitch : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("魔女の大釜", "Witch's Brew", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, false, card);
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

                            if (Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner && Unit.Character != card)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(UntilTurnEndAction);

                ICardEffect UntilTurnEndAction(EffectTiming _timing)
                {
                    if (_timing == EffectTiming.OnEndTurn)
                    {
                        ActivateClass activateClass1 = new ActivateClass();
                        activateClass1.SetUpICardEffect("カードを手札に加える", "Add a card\nto hand.", new List<Cost>() { new ReverseCost(1, (cardSource) => true)}, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true, card);
                        activateClass1.SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                        return activateClass1;

                        bool CanUseCondition1(Hashtable hashtable1)
                        {
                            if (card.UnitContainingThisCharacter() != null)
                            {
                                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                                {
                                    return true;
                                }
                            }

                            return false;
                        }

                        IEnumerator ActivateCoroutine1()
                        {
                            SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                            selectCardEffect.SetUp(
                                CanTargetCondition: (cardSource) => !cardSource.UnitNames.Contains("シェイド"),
                                CanTargetCondition_ByPreSelecetedList: null,
                                CanEndSelectCondition: null,
                                CanNoSelect: () => false,
                                SelectCardCoroutine: null,
                                AfterSelectCardCoroutine: null,
                                Message: "Select a card to add to hand.",
                                MaxCount: 1,
                                CanEndNotMax: false,
                                isShowOpponent: true,
                                mode: SelectCardEffect.Mode.AddHand,
                                root: SelectCardEffect.Root.Trash,
                                CustomRootCardList: null,
                                CanLookReverseCard: true,
                                SelectPlayer: card.Owner,
                                cardEffect: activateClass);

                            yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));
                        }
                    }

                    return null;
                }

                yield return null;
            }
        }

        return cardEffects;
    }
}

