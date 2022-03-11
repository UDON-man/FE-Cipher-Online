using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Jikbelt_DawnPrince : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("未来を繋ぐ盾", "", null, null, -1, false, card);
        canNotAttackClass.SetUpCanNotAttackClass(AttackingCondition, DefendingCondition);
        cardEffects.Add(canNotAttackClass);

        bool AttackingCondition(Unit AttackingUnit)
        {
            if (AttackingUnit != null)
            {
                if (AttackingUnit.Character != null)
                {
                    if (AttackingUnit.Character.Owner != card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool DefendingCondition(Unit DefendingUnit)
        {
            if (DefendingUnit != null && IsExistOnField(null, card))
            {
                if (DefendingUnit.Character != null)
                {
                    if (DefendingUnit != card.UnitContainingThisCharacter() && DefendingUnit.Character.cardColors.Contains(CardColor.White) && DefendingUnit.Character.Owner == card.Owner)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("君主の務め", "Burdens of a Monarch", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, 1, true, card);
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

                            if (Unit.Character != null)
                            {
                                if (Unit.Character.Owner == card.Owner && Unit.Character != card && Unit.Character.cardColors.Contains(CardColor.Black))
                                {
                                    return true;
                                }
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
                    CanTargetCondition: (unit) => unit.Character.Owner != card.Owner && unit.Character.Owner.GetBackUnits().Contains(unit),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
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