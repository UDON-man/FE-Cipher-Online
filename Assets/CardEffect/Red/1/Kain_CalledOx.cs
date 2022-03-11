using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class Kain_CalledOx : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        CanNotAttackClass canNotAttackClass = new CanNotAttackClass();
        canNotAttackClass.SetUpICardEffect("聖騎士の加護", "",null, null, -1, false,card);
        canNotAttackClass.SetUpCanNotAttackClass(AttackingCondition, DefendingCondition);
        cardEffects.Add(canNotAttackClass);

        bool AttackingCondition(Unit AttackingUnit)
        {
            if(AttackingUnit != null)
            {
                if(AttackingUnit.Character != null)
                {
                    if(AttackingUnit.Character.Owner != card.Owner && AttackingUnit.Character.Owner.GetBackUnits().Contains(AttackingUnit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool DefendingCondition(Unit DefendingUnit)
        {
            if(DefendingUnit != null)
            {
                if(DefendingUnit.Character != null)
                {
                    if(DefendingUnit.Character.Owner == card.Owner && (DefendingUnit == card.UnitContainingThisCharacter() || DefendingUnit.Character.cEntity_Base.PlayCost <= 2))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                SelectPlayer: card.Owner,
                CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit.Character.UnitNames.Contains("アベル") && !unit.IsTapped,
                CanTargetCondition_ByPreSelecetedList: null,
                CanEndSelectCondition: null,
                MaxCount: 1,
                CanNoSelect: false,
                CanEndNotMax: false,
                SelectUnitCoroutine: null,
                AfterSelectUnitCoroutine: null,
                mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("赤緑の双撃", "Red-Green Twin Strike",new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, true,card);
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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                Unit targetUnit = card.UnitContainingThisCharacter();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 40, (unit) => unit == targetUnit, true);

                targetUnit.UntilEndBattleEffects.Add((_timing) => powerUpClass);

                yield return null;
            }
        }

        return cardEffects;
    }
}
