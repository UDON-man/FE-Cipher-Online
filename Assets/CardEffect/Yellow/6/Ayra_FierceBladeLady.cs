using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Ayra_FierceBladeLady : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("オードの剣技", "Odo's Technique",new List<Cost>() { new ReverseSelfCost() }, null, -1, false,card);
            activateClass.SetUpActivateClass((hashtabl) => ActivateCoroutine());
            activateClass.SetBS();
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Character.Weapons.Contains(Weapon.Sword),
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
                    CanNotBeEvadedClass canNotBeEvadedClass = new CanNotBeEvadedClass();
                    canNotBeEvadedClass.SetUpCanNotBeEvadedClass((AttackingUnit) => AttackingUnit == unit, (DefendingUnit) => DefendingUnit != card.Owner.Enemy.Lord);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => canNotBeEvadedClass);

                    yield return null;
                }
            }
        }

        return cardEffects;
    }
}
