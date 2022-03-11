using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Rutger_RedRevengeDemon : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            int Cost()
            {
                if(IsExistOnField(null,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null && GManager.instance.turnStateMachine.DefendingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                        {
                            if (card.Owner.SupportCards.Count((cardSource) => cardSource.Weapons.Contains(Weapon.Sword)) > 0)
                            {
                                return 0;
                            }
                        }
                    }
                }

                return 1;
            }

            List<Cost> Costs = new List<Cost>();

            if(Cost() != 0)
            {
                Costs = new List<Cost>() { new ReverseCost(Cost(), (cardSource) => true) };
            }

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("血に飢えた剣", "Bloodthirsty Sword",Costs, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        return true;
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner.Lord != unit && unit.Character.Owner != card.Owner && unit.Character.cEntity_Base.PlayCost == 1,
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

        return cardEffects;
    }
}
