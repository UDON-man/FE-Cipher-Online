using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ryoma_LightningSwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("不屈の軍勢", "Unyielding Forces", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == card.Owner)
                {
                    return true;
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectCardEffect selectCardEffect = GetComponent<SelectCardEffect>();

                selectCardEffect.SetUp(
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    CanNoSelect: () => false,
                    SelectCardCoroutine: null,
                    AfterSelectCardCoroutine: null,
                    Message: "Select a card to deploy.",
                    MaxCount: 1,
                    CanEndNotMax: false,
                    isShowOpponent: true,
                    mode: SelectCardEffect.Mode.Deploy,
                    root: SelectCardEffect.Root.Trash,
                    CustomRootCardList: null,
                    CanLookReverseCard: true,
                    SelectPlayer: card.Owner,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectCardEffect.Activate(null));

                bool CanTargetCondition(CardSource cardSource)
                {
                    if (cardSource.Owner == card.Owner)
                    {
                        if (cardSource.cardColors.Contains(CardColor.White))
                        {
                            if (cardSource.cEntity_Base.PlayCost <= 2)
                            {
                                if (cardSource.CanPlayAsNewUnit())
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("雷神無双","", null, new List<Func<Hashtable, bool>>() , -1, false,card);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit == card.UnitContainingThisCharacter(), true);
        cardEffects.Add(powerUpClass);

        int ChangePower(Unit unit, int Power)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (unit == card.UnitContainingThisCharacter())
                    {
                        int count = 0;

                        foreach (Unit _unit in card.Owner.FieldUnit)
                        {
                            if (_unit != card.UnitContainingThisCharacter())
                            {
                                if (_unit.Character.cardColors.Contains(CardColor.White))
                                {
                                    count++;
                                }
                            }
                        }

                        if (count >= 4)
                        {
                            Power += 20;
                        }
                    }
                }
            }

            return Power;
        }

        RangeUpClass rangeUpClass = new RangeUpClass();
        rangeUpClass.SetUpICardEffect("雷神無双","", null, new List<Func<Hashtable, bool>>() , -1, false,card);
        rangeUpClass.SetUpRangeUpClass(ChangeRange, (unit) => unit == card.UnitContainingThisCharacter());
        cardEffects.Add(rangeUpClass);

        List<int> ChangeRange(Unit unit,List<int> Range)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                {
                    if (unit == card.UnitContainingThisCharacter())
                    {
                        int count = 0;

                        foreach(Unit _unit in card.Owner.FieldUnit)
                        {
                            if(_unit != card.UnitContainingThisCharacter())
                            {
                                if(_unit.Character.cardColors.Contains(CardColor.White))
                                {
                                    count++;
                                }
                            }
                        }

                        if(count >= 4)
                        {
                            Range.Add(1); 
                            Range.Add(2);
                        }
                    }
                }
            }

            return Range;
        }

        InvalidationClass invalidationClass = new InvalidationClass();
        invalidationClass.SetUpICardEffect("雷神無双","", null, new List<Func<Hashtable, bool>>(), -1, false,card);
        invalidationClass.SetUpInvalidationClass(InvalidateCondition);
        cardEffects.Add(invalidationClass);

        bool InvalidateCondition(ICardEffect cardEffect)
        {
            if (cardEffect.EffectName == "不屈の軍勢")
            {
                if (card.UnitContainingThisCharacter() != null)
                {
                    if (card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                    {
                        int count = 0;

                        foreach (Unit _unit in card.Owner.FieldUnit)
                        {
                            if (_unit != card.UnitContainingThisCharacter())
                            {
                                if (_unit.Character.cardColors.Contains(CardColor.White))
                                {
                                    count++;
                                }
                            }
                        }

                        if (count >= 4)
                        {
                            return true;
                        }
                    }
                }
            } 

            return false;
        }

        return cardEffects;
    }
}

