using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Holyn_WielderOfLuna : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("月光剣", "Luna",new List<Cost>() { new ReverseCost(2, (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit == card.Owner.Enemy.Lord)
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
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.PlayCost <= 3 && unit.Character.Owner != card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        PowerModifyClass powerUpClass = new PowerModifyClass();
        powerUpClass.SetUpICardEffect("孤剣の王者","", null, null, -1, false,card);
        powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, CanPowerUpCondition, true);
        cardEffects.Add(powerUpClass);

        bool CanPowerUpCondition(Unit unit)
        {
            if(IsExistOnField(null,card))
            {
                if (card.UnitContainingThisCharacter() == unit)
                {
                    if (card.Owner.GetFrontUnits().Count((_unit) => _unit != card.UnitContainingThisCharacter()) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        return cardEffects;
    }
}

