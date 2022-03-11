using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class KamuiOnnna_SelectedSword : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("血塗られた黒刃", "Blood-Stained Dark Sword",new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("カムイ(女)")) }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine1());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine1()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit != unit.Character.Owner.Lord,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 3,
                    CanNoSelect: true,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return StartCoroutine(selectUnitEffect.Activate(null));
            }

            ActivateClass activateClass1 = new ActivateClass();
            activateClass1.SetUpICardEffect("闇の行軍", "March of Darkness", new List<Cost>() { new ReverseCost(3, (cardSource) => true), new DiscardHandCost(1, (cardSource) => cardSource.UnitNames.Contains("カムイ(女)")) }, null, -1, false, card);
            activateClass1.SetUpActivateClass((hashtable) => ActivateCoroutine2());
            cardEffects.Add(activateClass1);

            IEnumerator ActivateCoroutine2()
            {
                foreach (Unit unit in card.Owner.FieldUnit)
                {
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 20, (_unit) => _unit == unit && unit.Character.cardColors.Contains(CardColor.Black), true);
                    unit.UntilOpponentTurnEndEffects.Add((_timing) => powerUpClass);
                }

                RangeUpClass rangeUpClass = new RangeUpClass();
                rangeUpClass.SetUpRangeUpClass((unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => rangeUpClass);

                yield return null;
            }

        }

        return cardEffects;
    }
}