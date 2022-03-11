using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Ike_BlueFlameHero : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            int Cost()
            {
                int _Cost = 5;

                if(card.UnitContainingThisCharacter() != null)
                {
                    _Cost -= card.UnitContainingThisCharacter().Characters.Count - 1;
                }

                if(_Cost < 1)
                {
                    _Cost = 1;
                }

                return _Cost;
            }

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("天空", "Aether", new List<Cost>() { new ReverseCost(Cost(), (cardSource) => true) }, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1,true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if(IsExistOnField(hashtable,card))
                {
                    if(GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if(GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            if(GManager.instance.turnStateMachine.DefendingUnit == card.Owner.Enemy.Lord)
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
                    CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.Owner != card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 2,
                    CanNoSelect: false,
                    CanEndNotMax: true,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    SelectUnitEffect.Mode.Destroy,
                    cardEffect: activateClass);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));

                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilOpponentTurnEndEffects.Add((_timing) => powerUpClass);
            }
        }

        return cardEffects;
    }
}

