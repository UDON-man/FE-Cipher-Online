using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Cynthia_JusticeWing : CEntity_Effect
{
    public override List<ICardEffect> CardEffects(EffectTiming timing)
    {
        List<ICardEffect> cardEffects = new List<ICardEffect>();

        if (timing == EffectTiming.OnCCAnyone)
        {
            activateClass[0].SetUpICardEffect("ヒーロー登場!", new List<Cost>(), new List<Func<Hashtable, bool>>() { CanUseCondition }, -1, false);
            activateClass[0].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[0]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[0].EffectName = "Your hero arrives!";
            }

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
                                return Unit.Character == this.card;
                            }

                        }
                    }
                }

                return false;
            }

            IEnumerator ActivateCoroutine()
            {
                PowerUpClass powerUpClass = new PowerUpClass();
                powerUpClass.SetUpPowerUpClass((unit, Power) => Power + 30, (unit) => unit == card.UnitContainingThisCharacter());
                card.UnitContainingThisCharacter().UntilEachTurnEndUnitEffects.Add(powerUpClass);

                activateClass[1].SetUpICardEffect("", new List<Cost>() , new List<Func<Hashtable, bool>>(), -1, true);
                activateClass[1].SetUpActivateClass((_hashtable) => ActivateCoroutine1());

                IEnumerator ActivateCoroutine1()
                {
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("cardEffect", activateClass[1]);
                    yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
                }

                if (activateClass[1].CanUse(null))
                {
                    yield return ContinuousController.instance.StartCoroutine(activateClass[1].Activate_Optional_Cost_Execute(null, "Do you move the unit?"));
                }
            }
        }

        else if (timing == EffectTiming.OnEndAttackAnyone)
        {
            activateClass[2].SetUpICardEffect("華麗な退場!", new List<Cost>(), new List<Func<Hashtable, bool>>() { (hashtable) => GManager.instance.turnStateMachine.AttackingUnit == card.UnitContainingThisCharacter() }, -1, true);
            activateClass[2].SetUpActivateClass((hashtable) => ActivateCoroutine());
            cardEffects.Add(activateClass[2]);

            if (ContinuousController.instance.language == Language.ENG)
            {
                activateClass[2].EffectName = "Now I gracefully depart!";
            }

            IEnumerator ActivateCoroutine()
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("cardEffect", activateClass[2]);
                yield return ContinuousController.instance.StartCoroutine(new IMoveUnit(new List<Unit>() { card.UnitContainingThisCharacter() }, true, hashtable).MoveUnits());
            }
        }

        return cardEffects;
    }
}



