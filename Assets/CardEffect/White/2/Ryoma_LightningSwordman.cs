using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ryoma_LightningSwordman : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnStartTurn)
        {
            activateClass[0].SetUpICardEffect("不屈の軍勢", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Unyielding Forces";
            }

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
                    CanLookReverseCard: true);

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

        PowerUpClass powerUpClass = new PowerUpClass();
        powerUpClass.SetUpICardEffect("雷神無双", null, new List<Func<Hashtable, bool>>() , -1, false);
        powerUpClass.SetUpPowerUpClass(ChangePower, (unit) => unit == card.UnitContainingThisCharacter());
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
        rangeUpClass.SetUpICardEffect("雷神無双", null, new List<Func<Hashtable, bool>>() , -1, false);
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

        //if(CanUseCondition(null))
        {
            InvalidationClass invalidationClass = new InvalidationClass();
            invalidationClass.SetUpICardEffect("雷神無双", null, new List<Func<Hashtable, bool>>() , -1, false);
            invalidationClass.SetUpInvalidationClass(InvalidateCondition);
            cardEffects.Add(invalidationClass);
        }

        bool CanUseCondition(Hashtable hashtable)
        {
            if (card.UnitContainingThisCharacter() != null)
            {
                if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
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

            return false;
        }

        bool InvalidateCondition(ICardEffect cardEffect)
        {
            if (cardEffect.EffectName == "不屈の軍勢"|| cardEffect.EffectName == "Unyielding Forces")
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

