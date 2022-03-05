using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ina_RedDragonGunshi : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
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

            activateClass[0].SetUpICardEffect("竜鱗の軍師", new List<Cost>() { new TapCost(), selectAllyCost }, new List<Func<Hashtable, bool>>(), -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Dragon Tactician";
            }

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
                    mode: SelectUnitEffect.Mode.Move);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}
