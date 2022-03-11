using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Belca_TheDeadly : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnDeclaration)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("乾坤一擲", "All-or-Nothing", new List<Cost>() { new ReverseCost(1, (cardSource) => true) }, null, 1, false,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                ActivateClass activateClass1 = new ActivateClass();
                activateClass1.SetUpICardEffect("このユニットを\n撃破する", "Destroy\nthis unit.", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
                activateClass1.SetUpActivateClass((hashtable1) => ActivateCoroutine1());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(UntilTurnEndAction);

                bool CanUseCondition(Hashtable hashtable1)
                {
                    if (card.UnitContainingThisCharacter() != null)
                    {
                        if (GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter())
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
                    hashtable1.Add("Unit", new Unit(card.UnitContainingThisCharacter().Characters));
                    yield return ContinuousController.instance.StartCoroutine(new IDestroyUnit(card.UnitContainingThisCharacter(), 1, BreakOrbMode.Hand, hashtable1).Destroy());
                }

                ICardEffect UntilTurnEndAction(EffectTiming _timing)
                {
                    if (_timing == EffectTiming.OnEndAttackAnyone)
                    {
                        return activateClass1;
                    }

                    return null;
                }

                yield return null;
            }
        }

        else if (timing == EffectTiming.OnDestroyedAnyone)
        {
            ActivateClass activateClass2 = new ActivateClass();
            activateClass2.SetUpICardEffect("デス・ゲイル", "Death Gale", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, true,card);
            activateClass2.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass2);

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
                                if (Unit.Character.Owner == card.Owner)
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
                    CanTargetCondition: (unit) => unit.Character.Owner == card.Owner,
                    CanTargetCondition_ByPreSelecetedList: null,
                    CanEndSelectCondition: null,
                    MaxCount: 1,
                    CanNoSelect: true,
                    CanEndNotMax: false,
                    SelectUnitCoroutine: null,
                    AfterSelectUnitCoroutine: null,
                    mode: SelectUnitEffect.Mode.Move,
                    cardEffect:activateClass2);

                yield return ContinuousController.instance.StartCoroutine(selectUnitEffect.Activate(null));
            }
        }

        return cardEffects;
    }
}

