using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Henley_SmileSorcerer : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            activateClass[0].SetUpICardEffect("スライム", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Mire";
            }

            bool CanUseCondition(Hashtable hashtable)
            {
                if(hashtable != null)
                {
                    if (hashtable.ContainsKey("Unit"))
                    {
                        if(hashtable["Unit"] is Unit)
                        {
                            Unit CCUnit = (Unit)hashtable["Unit"];

                            return CCUnit.Character.Owner == this.card.Owner && CCUnit != this.card.UnitContainingThisCharacter();
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
                        mode: SelectHandEffect.Mode.Discard);

                        yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                    }
                }
            }
        }

        else if (timing == EffectTiming.OnDeclaration)
        {
            activateClass[1].SetUpICardEffect("死の呪い", new List<Cost>() { new TapCost(),new ReverseCost(1, (cardSource) => true),new DestroySelfCost() }, null, -1, false);
            activateClass[1].SetUpActivateClass((hashtable) => ActivateCoroutine());
            activateClass[1].SetCCS(card.UnitContainingThisCharacter());
            cardEffects.Add(activateClass[1]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[1].EffectName = "Mortal Hex";
            }

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
                        mode: SelectHandEffect.Mode.Discard);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}