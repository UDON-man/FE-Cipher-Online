using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Oboro_AnnyaHunter : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("入れ替え", "Swap", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: CanTargetCondition,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect:activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                bool CanTargetCondition(Unit unit)
                {
                    if(unit.Character.Owner == card.Owner)
                    {
                        if(card.UnitContainingThisCharacter() != null)
                        {
                            if(card.Owner.GetFrontUnits().Contains(card.UnitContainingThisCharacter()))
                            {
                                if(card.Owner.GetBackUnits().Contains(unit))
                                {
                                    return true;
                                }
                            }

                            else if (card.Owner.GetBackUnits().Contains(card.UnitContainingThisCharacter()))
                            {
                                if (card.Owner.GetFrontUnits().Contains(unit))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    if(unit != null)
                    {
                        Hashtable hashtable = new Hashtable();
                        hashtable.Add("cardEffect", activateClass);
                        yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { unit,card.UnitContainingThisCharacter() }, true,hashtable).MoveUnits());
                    }
                }
            }

        }

        else if (timing == EffectTiming.OnMovedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("魔王演武", "Devilish Intetions", new List<Cost>() ,new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
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

                            if (Unit == card.UnitContainingThisCharacter())
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
