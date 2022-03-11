using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Cynthia_JusticeWing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing, CardSource card)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("ヒーロー登場!", "Your hero arrives!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false,card);
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
                                if(Unit.Character == card)
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
                PowerModifyClass powerUpClass = new PowerModifyClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter(), true);
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add((_timing) => powerUpClass);

                ActivateClass activateClass1 = new ActivateClass();
                activateClass1.SetUpICardEffect("","", new List<Cost>() , new List<Func<Hashtable, bool>>(), -1, true,card);
                activateClass1.SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                IEnumerator ActivateCoroutine1()
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass);
                    yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
                }

                if (activateClass1.CanUse(null))
                {
                    yield return ContinuousController.instance.StartCoroutine(activateClass1.Activate_Optional_Cost_Execute(null, "Do you move the unit?"));
                }
            }
        }

        else if (timing == EffectTiming.OnEndAttackAnyone)
        {
            ActivateClass activateClass = new ActivateClass();
            activateClass.SetUpICardEffect("華麗な退場!", "Now I gracefully depart!",new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() }, -1, true,card);
            activateClass.SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass);

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
            }
        }

        return cardEffects;
    }
}



