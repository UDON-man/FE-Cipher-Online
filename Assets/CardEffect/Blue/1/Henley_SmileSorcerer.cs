using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Henley_SmileSorcerer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("スライム", "Mire", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if(hashtable["Unit"] is Unit)
                        {
                            Unit Unit = (Unit)hashtable["Unit"];

                            if(Unit.Character != null)
                            {
                                if(Unit.Character.Owner == card.Owner && Unit != card.UnitContainingThisCharacter())
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
                List<Player> players = new List<Player>();
                players.Add(card.Owner);
                players.Add(card.Owner.Enemy);

                SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                foreach(Player player in players)
                {
                    if(player.HandCards.Count > 0)
                    {
                        selectHandEffect.SetUp(
                        SelectPlayer: player,
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
                        cardEffect:activateClass);

                        yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                    }
                }
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("死の呪い", "Mortal Hex", new List<Cost>() { new TapCost(),new ReverseCost(1, (cardSource) => true),new DestroySelfCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass.SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.Enemy.HandCards.Count > 0)
                {
                    SelectHandEffect selectHandEffect = GetComponent<SelectHandEffect>();

                    int maxCount = 2;

                    if(card.Owner.Enemy.HandCards.Count < maxCount)
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
        }

        return cardEffects;
    }
}