using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Kachua_FastWingWhiteKnight : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                 SelectPlayer: card.Owner,
                 CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("パオラ") || unit.Character.UnitNames.Contains("エスト")),
                 CanTargetCondition_ByPreSelecetedList: null,
                 CanEndSelectCondition: null,
                 MaxCount: 1,
                 CanNoSelect: false,
                 CanEndNotMax: false,
                 SelectUnitCoroutine: null,
                 AfterSelectUnitCoroutine: null,
                 mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("カチュアの手槍", "Catria's Spear", new List<Cost>() { new TapCost(), selectAllyCost }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && unit.Weapons.Contains(Weapon.Wing),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: (unit) => SelectUnitCoroutine(unit),
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Custom,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                IEnumerator SelectUnitCoroutine(Unit unit)
                {
                    RangeUpClass rangeUpClass = new RangeUpClass();
                    rangeUpClass.SetUpRangeUpClass((_unit, Range) => { Range.Add(1); Range.Add(2); return Range; }, (_unit) => _unit == unit);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => rangeUpClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}