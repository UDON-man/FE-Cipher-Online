using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Challotte_RoughGirlPower : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("高貴な方って素敵ですぅ！", "", null, null, -1, false, card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 70, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if (card.UnitContainingThisCharacter() == unit)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    if(card.Owner.FieldUnit.Count((_unit) => _unit.Character.PlayCost >= 5 && _unit.Character.sex.Contains(Sex.male)) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnEnterFieldAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("二人きりでお話ししましょ?", "Shall we have a one-on-one chat", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
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

                            if (Unit != null)
                            {
                                if (Unit.Character != null)
                                {
                                    if (Unit.Character.Owner == card.Owner)
                                    {
                                        if (Unit.Character.PlayCost >= 4 && Unit.Character.sex.Contains(Sex.male))
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
                StrikeModifyClass strikeModifyClass = new StrikeModifyClass();
                strikeModifyClass.SetUpStrikeModifyClass((unit, Strike) => 2, (unit) => unit == card.UnitContainingThisCharacter(), false);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => strikeModifyClass);

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("…ちょろ過ぎんだろ!", "...You're tomm simple minded!", new List<Cost>() { new DiscardHandCost(1, (cardSource) => cardSource.PlayCost <= 3 && cardSource.sex.Contains(Sex.male)) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable, card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if(hashtable != null)
                        {
                            if(hashtable.ContainsKey("Unit"))
                            {
                                if(hashtable["Unit"] is Unit)
                                {
                                    Unit Unit = (Unit)hashtable["Unit"];

                                    if(Unit != null)
                                    {
                                        if(Unit.Character != null)
                                        {
                                            if(Unit.Character == card.Owner.Lord.Character)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                if (card.Owner.Enemy.HandCards.Count >= 1)
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
        }

        return cardEffects;
    }
}
