using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Midoriko_MedicineResearcher : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("げんきが出るお薬", "Energizing Medicine", new List<Cost>() { new TapCost() }, null, -1, false, card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                SelectUnitEffect selectUnitEffect = GetComponent<SelectUnitEffect>();

                selectUnitEffect.SetUp(
                    SelectPlayer: card.Owner,
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner && unit != unit.Character.Owner.Lord,
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
                    PowerModifyClass powerUpClass = new PowerModifyClass();
                    powerUpClass.SetUpPowerUpClass((_unit, Power) => Power + 10, (_unit) => _unit == unit, true);
                    unit.UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                    ActivateClass activateClass1 = new ActivateClass();
                    activateClass1.SetUpICardEffect("このユニットを\n撃破する", "Destroy\nthis unit.", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false, card);
                    activateClass1.SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                    unit.UntilEachTurnEndUnitEffects.Add(UntilTurnEndAction);

                    bool CanUseCondition(Hashtable hashtable1)
                    {
                        if (unit != null)
                        {
                            if(unit.Character != null)
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    IEnumerator ActivateCoroutine1()
                    {
                        Hashtable hashtable1 = new Hashtable();
                        hashtable1.Add("cardEffect", activateClass1);
                        hashtable1.Add("Unit", new Unit(unit.Characters));
                        yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(unit, 1, BreakOrbMode.Hand, hashtable1).Destroy());
                    }

                    ICardEffect UntilTurnEndAction(EffectTiming _timing)
                    {
                        if (_timing == EffectTiming.OnEndTurn)
                        {
                            if(unit != null)
                            {
                                if(unit.Character != null)
                                {
                                    if (GManager.instance.turnStateMachine.gameContext.TurnPlayer == unit.Character.Owner)
                                    {
                                        return activateClass1;
                                    }
                                }
                            }
                        }

                        return null;
                    }

                    yield return null;
                }
            }

        }

        return cardEffects;
    }

    #region 援護の紋章
    public override List<ICardEffect> SupportEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> supportEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnEndAttackAnyone)
        {
            ActivateClass activateClass_Support = new ActivateClass();
            activateClass_Support.SetUpICardEffect("援護の紋章", "Flying Emblem", null, new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true, card);
            activateClass_Support.SetUpActivateClass((hashtable) => ActivateCoroutine());
            supportEffects.Add(activateClass_Support);

            bool CanUseCondition(Hashtable hashtable)
            {
                if (card.Owner.SupportCards.Contains(card))
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                        {
                            if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
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
                if (GManager.instance.turnStateMachine.AttackingUnit != null)
                {
                    if (GManager.instance.turnStateMachine.AttackingUnit.Character != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit.Character.Owner == card.Owner)
                        {
                            Hashtable hashtable = new Hashtable();
                            hashtable.Add("cardEffect", activateClass_Support);
                            yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { GManager.instance.turnStateMachine.AttackingUnit }, true, hashtable).MoveUnits());
                        }
                    }
                }
            }
        }

        return supportEffects;
    }
    #endregion
}
