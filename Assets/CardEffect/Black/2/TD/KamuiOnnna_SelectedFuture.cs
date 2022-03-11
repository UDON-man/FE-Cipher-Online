using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class KamuiOnnna_SelectedFuture : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnAttackAnyone)
        {
            SelectAllyCost selectAllyCost = new SelectAllyCost(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && !unit.IsTapped && (unit.Character.UnitNames.Contains("マークス") || unit.Character.UnitNames.Contains("カミラ") || unit.Character.UnitNames.Contains("レオン") || unit.Character.UnitNames.Contains("エリーゼ")),
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: false,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Tap);

            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("暗夜に捧ぐ剣", "Consecrated Sword of Nohr", new List<Cost>() { selectAllyCost }, new List<Func<Hashtable, bool>>() { CanUseCondition1 }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            bool CanUseCondition1(Hashtable hashtable)
            {
                if (IsExistOnField(hashtable,card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
                    {
                        if (GManager.instance.turnStateMachine.DefendingUnit != null)
                        {
                            if (GManager.instance.turnStateMachine.DefendingUnit.Character.cardColors.Contains(CardColor.White))
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
                PowerModifyClass powerUpClass1 = new PowerModifyClass();
                powerUpClass1.SetUpPowerUpClass((unit, Power) => Power + 10, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEndBattleEffects.Add((_timing) => powerUpClass1);
                yield return null;
            }
        }

        else if(timing == EffectTiming.OnDestroyDuringBattleAlly)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("黒き波動", "Dark Surge", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition } , -1, true,card);
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