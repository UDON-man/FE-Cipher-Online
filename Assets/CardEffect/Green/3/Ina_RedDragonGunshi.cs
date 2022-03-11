using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ina_RedDragonGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                 SelectPlayer: card.Owner,
                 CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != card.UnitContainingThisCharacter() && !unit.IsTapped && unit.Weapons.Contains(Weapon.Beast),
                 CanTargetCondition_ByPreSelecetedList: null,
                 CanEndSelectCondition: null,
                 MaxCount: 1,
                 CanNoSelect: false,
                 CanEndNotMax: false,
                 SelectUnitCoroutine: null,
                 AfterSelectUnitCoroutine: null,
                 mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("竜鱗の軍師", "Dragon Tactician",new List<Cost>() { new TapCost(), selectAllyCost }, new List<Func<Hashtable, bool>>(), -1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                int MaxCount = 1;
                bool CanEndNotMax = false;

                if (card.UnitContainingThisCharacter().IsLevelUp())
                {
                    MaxCount = 2;
                    CanEndNotMax = true;
                }

                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: MaxCount,
                    CanNoSelect: CanEndNotMax,
                    CanEndNotMax: CanEndNotMax,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}
