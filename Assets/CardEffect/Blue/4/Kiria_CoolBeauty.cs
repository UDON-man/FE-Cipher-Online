using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Kiria_CoolBeauty : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("幻想のステージ", "Fantasy Stage", new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (card.Owner.FieldUnit.Count((unit) => unit.Character.UnitNames.Contains("サーリャ")) > 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }   

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.Enemy.HandCards.Count > 0)
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
                            cardEffect:activateClass);

                    yield return ContinuousController.instance.StartCoroutine(selectHandEffect.Activate(null));
                }
            }
        }

        return cardEffects;
    }
}